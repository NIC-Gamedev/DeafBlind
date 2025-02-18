using System;
using System.Collections;
using System.Collections.Generic;
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
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, 5.0f);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, 50.0f);

        eventInstance.start();

        ThrowWave(pos, effectSource, soundObject);

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
                }
                return FMOD.RESULT.OK;
            }, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
        }
        
        eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, 5.0f);
        eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, 50.0f);

        eventInstance.start();

        ThrowWave(transform.pos, effectSource, soundObject);

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
    protected void ThrowWave(Vector3 pos, EventInstance eventInstance, GameObject soundObject = null)
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
    protected IEnumerator EmitWave(ParticleSystem particle, EventInstance audioSource, GameObject soundObject = null)
    {
        const int spectrumSize = 512;
        float[] spectrumData = new float[spectrumSize];
        if (audioSource.clip.name == "Microphone")
        {
            audioSource.outputAudioMixerGroup = voicesMixer;
            voicesMixer.audioMixer.SetFloat("VoiceVol", -80);
        }

        if (particle)
        {
            var mainModule = particle.main;
            float previousIntensity = 0f;
            var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
            particle.transform.SetParent(audioSource.transform);
            particle.transform.position = audioSource.transform.position;

            if (audioSource.name == Microphone.devices[0].ToString())
            {
                audioSource.mute = true;
            }

            while (audioSource.isPlaying || audioSource.loop)
            {
                if (particle == null)
                {
                    break;
                }

                psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(audioSource.maxDistance, audioSource.maxDistance, audioSource.maxDistance) * 2);
                audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

                float bassIntensity = CalculateBassIntensity(spectrumData, 128);


                float alpha = Mathf.Clamp01(bassIntensity * 10f);
                mainModule.startColor = new MinMaxGradient(new Color(1f, 1f, 1f, alpha));
                mainModule.startSize = Mathf.Lerp(0.1f, 5f, alpha);

                if (bassIntensity > previousIntensity * 1.2f)
                {
                    if (audioSource.loop)
                    {
                        mainModule.startLifetime = audioSource.maxDistance / 5;
                        mainModule.startSize = audioSource.maxDistance * 2;
                    }
                    var audioSourceColiders = Physics.OverlapSphere(audioSource.transform.position, audioSource.maxDistance, audioListenerLayer);
                    if (audioSourceColiders != null)
                    {
                        foreach (var item in audioSourceColiders)
                        {
                            if (soundObject == item.gameObject)
                                continue;

                            if (item.TryGetComponent(out IListenAudio listenerAudio))
                            {
                                var dist = Vector3.Distance(audioSource.transform.position, item.transform.position);
                                if (dist < listenerAudio.listenDistance)
                                    listenerAudio.OnListenAudio(audioSource.transform.position);
                            }
                        }
                    }
                    particle.Emit(1);
                }

                previousIntensity = bassIntensity;
                if (!audioSource.loop)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    mainModule.stopAction = ParticleSystemStopAction.None;
                    yield return null;
                }
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