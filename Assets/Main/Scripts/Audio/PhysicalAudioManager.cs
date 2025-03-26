using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
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
    
    public void CreateSoundByTransform(ref Sound sound,int numOfChannels, int sampleRate, ref CREATESOUNDEXINFO _exinfo, ChannelGroup channelGroup, Channel channel,Transform transform, bool playOnStart = true, float minDistance = 1, float maxDistance = 100)
    {
        _exinfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
        _exinfo.numchannels = numOfChannels;
        _exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        _exinfo.defaultfrequency = sampleRate;
        _exinfo.length = (uint)sampleRate * sizeof(short) * (uint)numOfChannels;
        FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(transform);
        RuntimeManager.CoreSystem.createSound(_exinfo.userdata, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref _exinfo, out sound);
        channel.set3DMinMaxDistance(minDistance, maxDistance);
        sound.set3DMinMaxDistance(minDistance, maxDistance);
        StartCoroutine(UpdateChannelPos(transform, channel));

        if (playOnStart)
        {
            PlayPhysSound(ref sound, ref channelGroup, ref channel,transform);
        }
    }

    private IEnumerator UpdateChannelPos(Transform target, Channel channel)
    {
        yield return new WaitForSeconds(1.2f);
        while (channel.isPlaying(out bool isPlaying) == FMOD.RESULT.OK && isPlaying)
        {
            FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(target);
            channel.set3DAttributes(ref attributes.position, ref attributes.velocity);
            yield return null; // Ждём один кадр
        }
    }

    public void PlayPhysSound(ref Sound sound, ref ChannelGroup channelGroup, ref Channel channel,Transform transform,bool isLoop = false)
    {
        RuntimeManager.CoreSystem.playSound(sound, channelGroup, true, out channel);
        ExecuteThrowWaveCO(transform, sound,ref channel,isLoop);
    }
    
    public EventInstance InstanceByPos(EventReference audioRef,Vector3 pos, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(audioRef);

        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
        eventInstance.setVolume(volume);
        eventInstance.setPitch(pitch);
        eventInstance.getChannelGroup(out var channelGroup2);
        channelGroup2.getNumDSPs(out var num);
        
        if (loop) //Кароче с лупами есть такой прикол что он вызвает утечку памяти
        {
            eventInstance.setCallback(
                (FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
                {
                    switch (type)
                    {
                        case EVENT_CALLBACK_TYPE.STOPPED:
                            _activeSounds.Add(eventInstance);
                            eventInstance.start();
                            ExecuteThrowWaveCO(pos, eventInstance);
                            break;
                        case EVENT_CALLBACK_TYPE.SOUND_PLAYED:
                            break;
                    }
                    return FMOD.RESULT.OK;
                },
                FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED
            );
        }

        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
        
        eventInstance.start();
        ExecuteThrowWaveCO(pos, eventInstance);
        
        return eventInstance;
    }
    private void ExecuteThrowWaveCO(Vector3 pos, EventInstance eventInstance)
    {
        DSP fft = default;
        StartCoroutine(TryGetChannelGroup(eventInstance, (channelGroup) =>
        {
            RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out fft);
            fft.setParameterInt((int)DSP_FFT.WINDOWSIZE, SpectrumSize * 2);
            channelGroup.addDSP(0, fft);
            ThrowInstanceWave(pos, eventInstance, fft,channelGroup);
        }));
    }
    
    private void ExecuteThrowWaveCO(Transform transform, Sound sound,ref Channel channel,bool isLoop = false)
    {
        DSP fft = default;
        StartCoroutine(IsChannelPlay(channel, (channel1 =>
        {
            RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out fft);
            fft.setParameterInt((int)DSP_FFT.WINDOWSIZE, SpectrumSize * 2);
            channel1.addDSP(0, fft);
            ThrowSoundWave(transform, sound, fft,channel1,isLoop:isLoop);
        })));
    }

    public IEnumerator TryGetChannelGroup(EventInstance eventInstance,System.Action<ChannelGroup> onSuccess)
    {
        ChannelGroup channelGroup;
        while (eventInstance.getChannelGroup(out channelGroup) != RESULT.OK)
        {
            yield return null;
        }
        onSuccess?.Invoke(channelGroup);
    }
    
    public IEnumerator IsChannelPlay(Channel channel,System.Action<Channel> onSuccess)
    {
        channel.getPaused(out var isPlaying);
        while (isPlaying == false)
        {
            Debug.Log("NotPlay");
            channel.isPlaying(out isPlaying);
            yield return null;
        }
        Debug.Log("Play");
        onSuccess?.Invoke(channel);
    }
    public EventInstance InstanceByTransform(EventReference audioRef,Transform transform, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(audioRef);
        
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        eventInstance.setVolume(volume);
        eventInstance.setPitch(pitch);
        if (loop)
        {
            eventInstance.setCallback(
                (FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) =>
                {
                    switch (type)
                    {
                        case EVENT_CALLBACK_TYPE.STOPPED:
                            _activeSounds.Add(eventInstance);
                            eventInstance.start();
                            ExecuteThrowWaveCO(transform.position, eventInstance);

                            break;
                        case EVENT_CALLBACK_TYPE.SOUND_PLAYED:

                            break;
                    }
                    return FMOD.RESULT.OK;
                },
                FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED
            );
        }

        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);

        eventInstance.start();
        
        ExecuteThrowWaveCO(transform.position, eventInstance);

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
    protected void ThrowInstanceWave(Vector3 pos, EventInstance eventInstance,DSP fft,ChannelGroup channelGroup, Transform soundObject = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, pos, Quaternion.identity);
        var main = instance.main;
        eventInstance.getMinMaxDistance(out var min, out var max);
        main.startSize = max;
        StartCoroutine(EmitWave(instance, eventInstance,fft, soundObject));
    }
    protected void ThrowSoundWave(Transform soundTransform, Sound sound,DSP fft,Channel channel, Transform soundObject = null,bool isLoop = false)
    {
        ParticleSystem instance = Instantiate(waveParticle, soundTransform.position, Quaternion.identity);
        var main = instance.main;
        sound.get3DMinMaxDistance (out var min, out var max);
        if (isLoop)
        {
            main.stopAction = ParticleSystemStopAction.None;
        }
        main.startSize = max;
        StartCoroutine(EmitWave(instance, sound,fft,channel,soundTransform, soundObject));
    }
    protected IEnumerator EmitWave(ParticleSystem particle, EventInstance eventInstance,DSP fft, Transform soundObject = null)
    {
            if (particle)
            {
                var mainModule = particle.main;
                float previousIntensity = 0f;
                float[] spectrumData = new float[SpectrumSize*2];
                var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
                eventInstance.getMinMaxDistance(out var min,out var max);
                eventInstance.getChannelGroup(out var channelGroup);

                IntPtr data;
                uint size;
                while (particle != null || eventInstance.hasHandle() || psRenderer != null)
                {
                    if (fft.getParameterData((int)DSP_FFT.SPECTRUMDATA, out data,out size) == RESULT.OK)
                    {
                        DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(FMOD.DSP_PARAMETER_FFT));
                        if (fftData.numchannels > 0 && fftData.spectrum.Length > 0)
                        {
                            fftData.spectrum[0].CopyTo(spectrumData, 0);
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                    }
                    if (!psRenderer)
                    {
                        break;
                    }
                    psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(min, max, max) * 2);

                    float bassIntensity = CalculateBassIntensity(spectrumData, SpectrumSize*2);
                    float alpha = Mathf.Clamp01(bassIntensity * 10f);
                    mainModule.startLifetime = Mathf.Pow(bassIntensity, 0.3f) * 5f;
                    mainModule.startColor = new MinMaxGradient(new Color(1f, 1f, 1f, alpha));
                    mainModule.startSize = new MinMaxCurve(min,Mathf.Lerp(min,max,bassIntensity));

                    if (bassIntensity >= previousIntensity * 1.2f)
                    {
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
                    yield return new WaitForSeconds(0.05f);
                }
                channelGroup.removeDSP(fft);
                fft.release();
                eventInstance.release();  
            }
    }
    
        protected IEnumerator EmitWave(ParticleSystem particle, Sound sound,DSP fft,Channel channel,Transform soundTransform, Transform soundObject = null)
    {
            if (particle)
            {
                var mainModule = particle.main;
                float previousIntensity = 0f;
                float[] spectrumData = new float[SpectrumSize*2];
                var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
                sound.get3DMinMaxDistance(out var min,out var max);

                IntPtr data;
                uint size;
                while (particle != null || sound.hasHandle() || psRenderer != null)
                {
                    if (fft.getParameterData((int)DSP_FFT.SPECTRUMDATA, out data,out size) == RESULT.OK)
                    {
                        DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(FMOD.DSP_PARAMETER_FFT));
                        if (fftData.numchannels > 0 && fftData.spectrum.Length > 0)
                        {
                            fftData.spectrum[0].CopyTo(spectrumData, 0);
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                    }
                    
                    if (!psRenderer)
                    {
                        break;
                    }
                    
                    psRenderer.transform.position = soundTransform.position;
                    psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(min, max, max) * 2);

                    float bassIntensity = CalculateBassIntensity(spectrumData, SpectrumSize*2);
                    float alpha = Mathf.Clamp01(bassIntensity * 10f);
                    mainModule.startLifetime = Mathf.Pow(bassIntensity, 0.3f) * 5f;
                    mainModule.startColor = new MinMaxGradient(new Color(1f, 1f, 1f, alpha));
                    
                    mainModule.startSize = new MinMaxCurve(min,Mathf.Lerp(min,max,bassIntensity));

                    if (bassIntensity >= previousIntensity * 1.2f)
                    {
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
                    yield return new WaitForSeconds(0.05f);
                }
                channel.removeDSP(fft);
                fft.release();
                sound.release();  
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