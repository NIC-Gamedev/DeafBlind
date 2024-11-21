using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class AudioManager : MonoBehaviour
{
    private const string SFX_PARENT_NAME = "SFX";
    private const string SFX_NAME_FORMAT = "SFX - [{0}]";

    public const float TRACK_TRANSITION_SPEED = 1f;    public static AudioManager instance { get; private set; }

    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voicesMixer;

    [SerializeField] protected ParticleSystem waveParticle;

    private Transform sfxRoot;



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
        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        PlaySoundEffect(audioClip,position:null);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        PlayTrack(audioClip, pos:null);
    }
    public void StopMusic(AudioClip audioClip)
    {
        string audioName = audioClip.name;
        StopTrack(audioName);
    }
    public void StopMusic(int channel)
    {
        StopTrack(channel);
    }
    public void StopMusic(string audioName)
    {
        StopTrack(audioName);
    }
    public AudioSource PlaySoundEffect(string filepath,AudioMixerGroup mixer = null,float volume = 1,float pitch = 1,bool loop = false, float minDistance = 1, float maxDistance = 100, Collision ColideObject = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filepath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filepath}'. Please make sure this exist audio");
            return null;
        }
        return PlaySoundEffect(clip,mixer,volume,pitch,loop, minDistance,maxDistance, ColideObject);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false,float minDistance = 1,float maxDistance = 100, Collision ColideObject = null)
    {
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();

        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = ColideObject == null ? sfxRoot.position : ColideObject.contacts[0].point;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch = pitch;
        effectSource.loop = loop;
        effectSource.minDistance = minDistance;
        effectSource.maxDistance = maxDistance;
        effectSource.spatialBlend = 1;

        effectSource.Play();

        if (ColideObject != null)
            ThrowWave(ColideObject, effectSource);

        if (!loop)
            Destroy(effectSource.gameObject,(clip.length / pitch) + 1);

        return effectSource;
    }
    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Transform position = null)
    {
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();

        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = position == null ? sfxRoot.position : position.position;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch = pitch;
        effectSource.loop = loop;
        effectSource.minDistance = minDistance;
        effectSource.maxDistance = maxDistance;
        effectSource.spatialBlend = 1;

        effectSource.Play();

        if (position != null)
            ThrowWave(position, effectSource);

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }


    public AudioSource PlayVoice(string filepath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filepath, voicesMixer, volume, pitch, loop);
    }
    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, voicesMixer, volume, pitch, loop,position:null);
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip.name.ToLower() == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true,float startingVolume = 0f,float volumeCap = 1f,float pitch = 1f, float minDistance = 1, float maxDistance = 100, Collider collider = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap,pitch, filePath,minDistance,maxDistance,collider);
    }
    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, float minDistance = 1, float maxDistance = 100, Transform pos = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath, minDistance, maxDistance, pos);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f,float pitch = 1f,string filePath = "", float minDistance = 1, float maxDistance = 100, Collider collider = null)
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfNotExists:true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);

        track.source.maxDistance = maxDistance;
        track.source.minDistance = minDistance;

        track.source.transform.SetParent(sfxRoot);
        track.source.transform.position = sfxRoot.position;

        track.source.clip = clip;
        track.source.pitch = pitch;
        track.source.loop = loop;
        track.source.minDistance = minDistance;
        track.source.maxDistance = maxDistance;
        track.source.spatialBlend = 1;
        track.source.transform.position = collider.transform.position;
        track.source.transform.SetParent(collider.transform);
        track.source.dopplerLevel = 0.1f;

        if (collider != null)
            ThrowWave(collider,track.source);

        return track;
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "", float minDistance = 1, float maxDistance = 100, Transform pos = null)
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfNotExists: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);

        track.source.maxDistance = maxDistance;
        track.source.minDistance = minDistance;

        track.source.transform.SetParent(sfxRoot);
        track.source.transform.position = sfxRoot.position;

        track.source.clip = clip;
        track.source.pitch = pitch;
        track.source.loop = loop;
        track.source.minDistance = minDistance;
        track.source.maxDistance = maxDistance;
        track.source.spatialBlend = 1;
        track.source.dopplerLevel = 0.1f;

        if (pos != null)
        {
            track.source.transform.position = pos.transform.position;
            track.source.transform.SetParent(pos.transform);

            ThrowWave(pos:pos, audioSource:track.source);
        }

        return track;
    }

    public void StopTrack(int channel)
    {
        AudioChannel c = TryGetChannel(channel, createIfNotExists: false);

        if (c == null)
            return;

        c.StopTrack();
    }
    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach(var channel in channels.Values)
        {
            if (channel.activeTrack != null &&  channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public AudioChannel TryGetChannel(int channelNumber,bool createIfNotExists = false)
    {
        AudioChannel channel = null;

        if (channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if (createIfNotExists)
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }
        return null;
    }

    protected void ThrowWave(Collision collision,AudioSource audioSource = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, collision.contacts[0].point, Quaternion.identity);
        var main = instance.main;

        main.startLifetime = audioSource.clip.length;
        main.startSize = audioSource.maxDistance;
        StartCoroutine(EmitWave(instance, audioSource));

    }
    protected void ThrowWave(Transform pos, AudioSource audioSource = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, pos.position, Quaternion.identity);
        var main = instance.main;

        main.startLifetime = audioSource.clip.length;
        main.startSize = audioSource.maxDistance;
        StartCoroutine(EmitWave(instance, audioSource));

    }
    protected void ThrowWave(Collider collider, AudioSource audioSource = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, collider.transform.position, Quaternion.identity);
        var main = instance.main;
        instance.transform.SetParent(collider.transform);

        StartCoroutine(EmitWave(instance, audioSource));
    }
    protected IEnumerator EmitWave(ParticleSystem particle, AudioSource audioSource)
    {
        const int spectrumSize = 512;
        float[] spectrumData = new float[spectrumSize];
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

            while (audioSource.isPlaying)
            {
                if (particle == null)
                {
                    break;
                }


                psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(audioSource.maxDistance, audioSource.maxDistance, audioSource.maxDistance) * 2);
                audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

                float bassIntensity = CalculateBassIntensity(spectrumData,128);


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
