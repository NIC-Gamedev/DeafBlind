using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FishNet.Object;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager instance { get; private set; }
    
    [SerializeField] private List<EventInstance> _activeSounds = new List<EventInstance>();

    private void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public void CreateSound(out Sound sound, int numOfChannels, int sampleRate, ref CREATESOUNDEXINFO _exinfo, ChannelGroup channelGroup, Channel channel, bool playOnStart = true)
    {
        _exinfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
        _exinfo.numchannels = numOfChannels;
        _exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        _exinfo.defaultfrequency = sampleRate;
        _exinfo.length = (uint)sampleRate * sizeof(short) * (uint)numOfChannels;
        RuntimeManager.CoreSystem.createSound(_exinfo.userdata, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref _exinfo, out sound);

        if (playOnStart)
        {
            PlaySound(sound, channelGroup, out channel);
        }
    }
    public unsafe RESULT CreateSound(
        short[] pcmData,
        out Sound sound,
        int numOfChannels,
        int sampleRate,
        ref CREATESOUNDEXINFO _exinfo,
        ChannelGroup channelGroup,
        Channel channel,
        bool playOnStart = true
    )
    {
        _exinfo.cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO));
        _exinfo.numchannels = numOfChannels;
        _exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        _exinfo.defaultfrequency = sampleRate;
        _exinfo.length = (uint)(pcmData.Length * sizeof(short)); // Общий размер в байтах

        // Фиксируем массив в памяти, чтобы получить указатель
        fixed (short* pData = pcmData)
        {
            // Создаем звук из сырых PCM-данных
            FMOD.MODE mode = FMOD.MODE.OPENUSER | FMOD.MODE.OPENMEMORY | FMOD.MODE.OPENRAW | MODE.CREATESTREAM;
            FMOD.RESULT result = RuntimeManager.CoreSystem.createSound(
                (IntPtr)pData, // Указатель на данные
                mode,
                ref _exinfo,
                out sound
            );
            
            return result;
        }

        // Воспроизводим звук, если нужно
        if (playOnStart)
            RuntimeManager.CoreSystem.playSound(sound, channelGroup, false, out channel);
    }
    
    public RESULT PlaySound(Sound sound, ChannelGroup channelGroup, out Channel channel, bool startPaused = false)
    {
        // Воспроизводим звук (без паузы, если startPaused = false)
        FMOD.RESULT result = RuntimeManager.CoreSystem.playSound(
            sound,
            channelGroup,
            startPaused,
            out channel
        );
        return result;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaySoundSyncServerRpc(EventReference audioRef, float volume = 1, float pitch = 1, bool loop = false)
    {
        PlaySoundSyncClientRpc(audioRef, volume, pitch, loop);
    }

    [ObserversRpc]
    private void PlaySoundSyncClientRpc(EventReference audioRef, float volume, float pitch, bool loop)
    {
        PlayInstance(audioRef, volume, pitch, loop);
    }
    public void PlayInstance(EventReference audioRef)
    {
        PlayInstance(audioRef,1,1,false);
    }

    public EventInstance PlayInstance(EventReference audioRef, float volume = 1, float pitch = 1, bool loop = false)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(audioRef);
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform));
        eventInstance.setVolume(volume);
        eventInstance.setPitch(pitch);
        
        if (loop)
        {
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
        
        eventInstance.start();
        if (!loop)
        {
            eventInstance.release();
        }
        else
        {
            _activeSounds.Add(eventInstance);
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
}