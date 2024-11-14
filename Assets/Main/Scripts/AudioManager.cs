using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class AudioManager : MonoBehaviour
{
    private const string SFX_PARENT_NAME = "SFX";
    private const string SFX_NAME_FORMAT = "SFX - [{0}]";

    public const float TRACK_TRANSITION_SPEED = 1f;
    public static AudioManager instance { get; private set; }

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
        PlaySoundEffect(audioClip);
    }

    public void PlayMusic(AudioClip audioClip)
    {
        PlayTrack(audioClip);
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
            ThrowWave(ColideObject, clip.length, Mathf.Lerp(0.5f, 25f, volume), (int)Mathf.Lerp(1, 10, clip.length), effectSource);

        if (!loop)
            Destroy(effectSource.gameObject,(clip.length / pitch) + 1);

        return effectSource;
    }


    public AudioSource PlayVoice(string filepath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filepath, voicesMixer, volume, pitch, loop);
    }
    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, voicesMixer, volume, pitch, loop);
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

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true,float startingVolume = 0f,float volumeCap = 1f,float pitch = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap,pitch, filePath);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f,float pitch = 1f,string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfNotExists:true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
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



    protected void ThrowWave(Collision collision, float startLifeTime = 1, float startSize = 1, int emitCount = 1,AudioSource audioSource = null)
    {
        ParticleSystem instance = Instantiate(waveParticle, collision.contacts[0].point, Quaternion.identity);
        var main = instance.main;
        main.startLifetime = startLifeTime;
        main.startSize = startSize;
        if(audioSource.loop) 
            StartCoroutine(UpdateParticleBasedOnAudio(instance,audioSource, emitCount));
        else
        {
            main.startLifetime = audioSource.clip.length;
            main.startSize = audioSource.maxDistance;
            StartCoroutine(EmitWave(instance,audioSource));
        }
    }
    protected IEnumerator UpdateParticleBasedOnAudio(ParticleSystem particle, AudioSource audioSource,int emitCount)
    {
        var main = particle.main;
        float[] spectrumData = new float[512];
        float previousIntensity = 0f;
        float threshold = 0.1f;
        float silenceThreshold = 0.01f;
        float multiplier = 100f; 
        float silenceTime = 0f;

        while (audioSource.isPlaying)
        {
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            float currentIntensity = 0f;

            for (int i = 0; i < spectrumData.Length; i++)
            {
                currentIntensity += spectrumData[i];
            }
            currentIntensity *= multiplier;

            if (currentIntensity < silenceThreshold)
            {
                silenceTime += Time.deltaTime;
            }
            else
            {
                silenceTime = 0f;
            }

            if (Mathf.Abs(currentIntensity - previousIntensity) > threshold)
            {
                main.startSize = Mathf.Lerp(0.5f, 25f, currentIntensity);
                main.startLifetime = Mathf.Lerp(0.5f, 5f, currentIntensity);
                particle.Emit(1);
            }

            if (silenceTime > 0.5f)
            {
                silenceTime = 0f; 
            }

            previousIntensity = currentIntensity;

            yield return null;
        }
    }
    protected IEnumerator EmitWave(ParticleSystem particle,AudioSource audioSource)
    {
        float[] spectrumData = new float[512];
        var mainModule = particle.main;
        while (audioSource.isPlaying)
        {
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            float currentIntensity = 0f;
            for (int i = 0; i < spectrumData.Length; i++)
            {
                currentIntensity += spectrumData[i];
            }

            currentIntensity *= 100f; 

            float alpha = Mathf.Clamp01(currentIntensity);

            Color startColor = new Color(1f, 1f, 1f, alpha);
            mainModule.startColor = startColor;
            mainModule.startSize = Mathf.Lerp(audioSource.minDistance, audioSource.maxDistance, currentIntensity);
            var psRenderer = particle.GetComponent<ParticleSystemRenderer>();
            psRenderer.bounds = new Bounds(psRenderer.transform.position, new Vector3(audioSource.maxDistance, audioSource.maxDistance, audioSource.maxDistance));

            particle.Emit(1);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
