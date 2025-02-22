using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using Debug = UnityEngine.Debug;

public class PhysicalAudioManager : MonoBehaviour
{
    public static PhysicalAudioManager instance { get; private set; }
    
    [SerializeField] private List<EventInstance> _activeSounds = new List<EventInstance>();
    
    [SerializeField] protected ParticleSystem waveParticle;

    public LayerMask audioListenerLayer;
    
    private const int SpectrumSize = 512;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    public EventInstance PlayByPos(EventReference audioRef, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Vector3 pos = default)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(audioRef);
        
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
        eventInstance.setVolume(volume);
        eventInstance.setPitch(pitch);
        

        // Создаём DSP для FFT анализа
        FMOD.ChannelGroup channelGroup;
        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out var fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, SpectrumSize * 2);
        RuntimeManager.CoreSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);
        channelGroup.addDSP(0, fft);

        // Добавляем DSP в ChannelGroup перед стартом!
        
        if (loop)
        {
            _activeSounds.Add(eventInstance);
            eventInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
            {
                if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
                {
                    EventInstance instance = new EventInstance(instancePtr);
                    instance.start();
                    ThrowWave(transform.position, eventInstance,fft, transform);
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

        eventInstance.start();
        ThrowWave(pos, eventInstance,fft);

        if (!loop)
            eventInstance.release();

        return eventInstance;
    }
    public EventInstance PlayByTransform(EventReference audioRef, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100,Transform transform = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(audioRef);
        
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        eventInstance.setVolume(volume);
        eventInstance.setPitch(pitch);
        
        eventInstance.getChannelGroup(out var channelGroup);

        // Создаём DSP для FFT анализа
        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out var fft);
        fft.setParameterInt((int)DSP_FFT.WINDOWSIZE, 1024); // Окно 1024
        fft.setActive(true); // Включаем DSP

        // Добавляем DSP в ChannelGroup перед стартом!
        channelGroup.addDSP(0, fft);
        
        if (loop)
        {
            _activeSounds.Add(eventInstance);
            eventInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
            {
                if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
                {
                    EventInstance instance = new EventInstance(instancePtr);
                    instance.start();
                    ThrowWave(transform.position, eventInstance,fft, transform);
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

        eventInstance.start();

        ThrowWave(transform.position, eventInstance,fft, transform);

        if (!loop)
        {
            eventInstance.release();
        }

        return eventInstance;
    }
    
    public void Stop(EventReference reference)
    {
        for (int i = _activeSounds.Count - 1; i >= 0; i--)
        {
            FMOD.Studio.EventInstance effectInstance = _activeSounds[i];
            
            effectInstance.getDescription(out FMOD.Studio.EventDescription eventDescription);
            eventDescription.getPath(out string path);
            
            if (path.Contains(reference.Path))
            {
                effectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                effectInstance.release();
                _activeSounds.RemoveAt(i);
            }
        }
    }
    protected void ThrowWave(Vector3 pos, EventInstance eventInstance,DSP fft, Transform soundObject = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, pos, Quaternion.identity);
        var main = instance.main;
        eventInstance.getDescription(out EventDescription eventDescription);
        eventDescription.loadSampleData();
        eventDescription.getLength(out var length);
        Debug.Log($"Length: {length}");
        main.startLifetime = (float)length;
        eventInstance.getMinMaxDistance(out var min, out var max);
        main.startSize = max;
        StartCoroutine(EmitWave(instance, eventInstance,fft, soundObject));
    }
    protected IEnumerator EmitWave(ParticleSystem particle, EventInstance eventInstance,DSP fft, Transform soundObject = null)
    {

        if (particle)
        {
            var mainModule = particle.main;
            float previousIntensity = 0f;
            float[] spectrumData = new float[SpectrumSize*2];
            var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
            eventInstance.getPlaybackState(out var playbackState);
            eventInstance.getMinMaxDistance(out var min,out var max);

            while (playbackState == PLAYBACK_STATE.PLAYING || playbackState == PLAYBACK_STATE.STARTING)
            {
                eventInstance.getPlaybackState(out var newPlaybackState);
                playbackState = newPlaybackState;
                if (particle == null)
                {
                    break;
                }
                
                if (fft.getParameterData((int)DSP_FFT.SPECTRUMDATA, out var data,out var size) == RESULT.OK)
                {
                    DSP_PARAMETER_FFT fftData = Marshal.PtrToStructure<DSP_PARAMETER_FFT>(data);
                    Debug.Log($"FFT Data Size: {size}");
                    Debug.Log($"FFT Channels: {fftData.numchannels}");
                    if (fftData.numchannels > 0 && fftData.spectrum.Length > 0)
                    {
                        fftData.spectrum[0].CopyTo(spectrumData, 0);
                    }
                }
                
                psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(min, max, max) * 2);

                float bassIntensity = CalculateBassIntensity(spectrumData, 128);

                float alpha = Mathf.Clamp01(bassIntensity * 10f);
                mainModule.startColor = new MinMaxGradient(new Color(1f, 1f, 1f, alpha));
                mainModule.startSize = Mathf.Lerp(0.1f, 5f, alpha);

                if (bassIntensity > previousIntensity * 1.2f)
                {
                    /*if (eventInstance.loop)
                    {
                        mainModule.startLifetime = eventInstance.maxDistance / 5;
                        mainModule.startSize = eventInstance.maxDistance * 2;
                    }*/
                    eventInstance.getDescription(out EventDescription eventDescription);
                    eventDescription.loadSampleData();
                    eventDescription.getLength(out var length);
                    Debug.Log($"Length: {length}");
                    mainModule.startLifetime = (float)length;
                    var audioSourceColiders = Physics.OverlapSphere(particle.transform.position, max, audioListenerLayer);
                    if (audioSourceColiders != null)
                    {
                        foreach (var item in audioSourceColiders)
                        {
                            if (soundObject == item.gameObject)
                                continue;

                            if (item.TryGetComponent(out IListenAudio listenerAudio))
                            {
                                var dist = Vector3.Distance(particle.transform.position, item.transform.position);
                                if (dist < listenerAudio.listenDistance)
                                    listenerAudio.OnListenAudio(particle.transform.position);
                            }
                        }
                    }
                    particle.Emit(1);
                }

                previousIntensity = bassIntensity;
                yield return new WaitForSeconds((0.1f));
            }
        }
    }

    private unsafe float CalculateBassIntensity(float[] spectrumData, int length)
    {
        float bassIntensity = 0f;
        fixed (float* spectrumPtr = spectrumData)
        {
            for (int i = 0; i < length; i++)
            {
                bassIntensity += *(spectrumPtr + i);
            }
        }
        return bassIntensity;
    }

}