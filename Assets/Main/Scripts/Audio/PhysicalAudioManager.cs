using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.ParticleSystem;

public class PhysicalAudioManager : MonoBehaviour
{

    private const string SFX_PARENT_NAME = "SFX";
    private const string SFX_NAME_FORMAT = "SFX - [{0}]";

    public const float TRACK_TRANSITION_SPEED = 1f; 
    public static PhysicalAudioManager instance { get; private set; }

    public Dictionary<GameObject, AudioSource> musics = new Dictionary<GameObject, AudioSource>();

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

    public AudioSource PlaySoundEffect(string filepath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Collision ColideObject = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filepath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filepath}'. Please make sure this exist audio");
            return null;
        }
        return PlaySoundEffect(clip, mixer, volume, pitch, loop, minDistance, maxDistance, ColideObject);
    }
    public AudioSource PlaySoundEffect(string filepath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Transform transforn = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filepath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filepath}'. Please make sure this exist audio");
            return null;
        }
        return PlaySoundEffect(clip, mixer, volume, pitch, loop, minDistance, maxDistance, transforn);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Collision ColideObject = null)
    {
        if (ColideObject == null)
        {
            Debug.LogError("Null reference in Physical sound, please add collision");
            return null;
        }
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();

        effectSource.transform.position = ColideObject.contacts[0].point;
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

        ThrowWave(ColideObject, effectSource);

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }
    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 1, float maxDistance = 100, Transform transform = null)
    {
        if (transform == null)
        {
            Debug.LogError("Null reference in Physical sound, please add collision");
            return null;
        }
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();

        effectSource.transform.position = transform.position;
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

        ThrowWave(transform, effectSource);

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, float minDistance = 0, float maxDistance = 10, Transform transform = null)
    {
        return PlaySoundEffect(clip, voicesMixer, volume, pitch, loop, transform: transform);
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

    public AudioSource PlayMusic(string filePath, int channel = 0, bool loop = true, float volume = 1f, float pitch = 1f, float minDistance = 1, float maxDistance = 100, Transform pos = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory");
            return null;
        }

        return PlayMusic(clip, channel, loop, volume, pitch, filePath, minDistance, maxDistance, pos);
    }

    public AudioSource PlayMusic(AudioClip clip, int channel = 0, bool loop = true, float volume = 1f, float pitch = 1f, string filePath = "", float minDistance = 1, float maxDistance = 100, Transform pos = null)
    {
        if (pos == null)
        {
            Debug.LogError("Null reference in Physical sound, please add transform");
            return null;
        }
        AudioSource source = new GameObject(string.Format(pos.gameObject.name, clip.name)).AddComponent<AudioSource>();

        source.transform.position = pos.transform.position;
        source.transform.SetParent(pos.transform);
        source.clip = clip;

        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.spatialBlend = 1;
        source.volume = volume;
        source.loop = loop;
        source.dopplerLevel = 0.1f;
        source.Play();

        if (!musics.ContainsKey(transform.gameObject))
            musics.Add(transform.gameObject,source);
        else
        {
            musics.Remove(transform.gameObject);
        }

        ThrowWave(pos: pos, audioSource: source);

        return source;
    }

    public void StopMusic(GameObject gameObject)
    {
        if (musics.ContainsKey(gameObject))
        {
            musics.Remove(gameObject);
        }
    }
    protected void ThrowWave(Collision collision, AudioSource audioSource = null)
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
    protected IEnumerator EmitWave(ParticleSystem particle, AudioSource audioSource)
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