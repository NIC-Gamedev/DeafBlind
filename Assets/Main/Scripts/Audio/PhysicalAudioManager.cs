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

public class PhysicalAudioManager : MonoBehaviour
{
    public static PhysicalAudioManager instance { get; private set; }
    
    [SerializeField] private List<EventInstance> _activeSounds = new List<EventInstance>();
    
    [SerializeField] protected ParticleSystem waveParticle;

    public LayerMask audioListenerLayer;

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
        if (loop)
        {
            _activeSounds.Add(eventInstance);
            eventInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
            {
                if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
                {
                    EventInstance instance = new EventInstance(instancePtr);
                    instance.start();
                    ThrowWave(transform.position, eventInstance, transform);
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

        eventInstance.start();

        ThrowWave(pos, eventInstance);

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
        if (loop)
        {
            _activeSounds.Add(eventInstance);
            eventInstance.setCallback((FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
            {
                if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED)
                {
                    EventInstance instance = new EventInstance(instancePtr);
                    instance.start();
                    ThrowWave(transform.position, eventInstance, transform);
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

        eventInstance.start();

        ThrowWave(transform.position, eventInstance, transform);

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
    protected void ThrowWave(Vector3 pos, EventInstance eventInstance, Transform soundObject = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, pos, Quaternion.identity);
        var main = instance.main;
        eventInstance.getDescription(out EventDescription eventDescription);
        eventDescription.getLength(out var length);
        main.startLifetime = (float)length/1000f;
        eventInstance.getMinMaxDistance(out var min, out var max);
        main.startSize = max;
        StartCoroutine(EmitWave(instance, eventInstance, soundObject));
    }
    protected IEnumerator EmitWave(ParticleSystem particle, EventInstance eventInstance, Transform soundObject = null)
    {
        /*if (audioSource.clip.name == "Microphone")
        {
            audioSource.outputAudioMixerGroup = voicesMixer;
            voicesMixer.audioMixer.SetFloat("VoiceVol", -80);
        }*/

        if (particle)
        {
            var mainModule = particle.main;
            float previousIntensity = 0f;
            const int spectrumSize = 512;
            float[] spectrumData = new float[spectrumSize];
            var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
            eventInstance.getPlaybackState(out var playbackState);
            eventInstance.getMinMaxDistance(out var min,out var max);
            eventInstance.getChannelGroup(out var channelGroup);
            RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out  var fft);
            fft.setParameterInt((int)DSP_FFT.WINDOWSIZE, spectrumSize);
            channelGroup.addDSP(0, fft);

            /*if (eventInstance.name == Microphone.devices[0].ToString())
            {
                eventInstance.mute = true;
            }*/

            while (playbackState == PLAYBACK_STATE.PLAYING)
            {
                if (particle == null)
                {
                    break;
                }

                psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(min, max, max) * 2);

                float bassIntensity = CalculateBassIntensity(spectrumData, 128);
                
                if (fft.getParameterData((int)DSP_FFT.SPECTRUMDATA, out var data,out var size) == RESULT.OK)
                {
                    DSP_PARAMETER_FFT fftData = Marshal.PtrToStructure<DSP_PARAMETER_FFT>(data);
                    if (fftData.numchannels > 0)
                    {
                        fftData.spectrum[0].CopyTo(spectrumData, 0);
                    }
                }

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
                yield return null;
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