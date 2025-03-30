using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    
    [SerializeField] private List<EventInstance> _activeSounds = new List<EventInstance>();

    private void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public void CreateSound(ref Sound sound, int numOfChannels, int sampleRate, ref CREATESOUNDEXINFO _exinfo, ChannelGroup channelGroup, Channel channel, bool playOnStart = true)
    {
        _exinfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
        _exinfo.numchannels = numOfChannels;
        _exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        _exinfo.defaultfrequency = sampleRate;
        _exinfo.length = (uint)sampleRate * sizeof(short) * (uint)numOfChannels;
        RuntimeManager.CoreSystem.createSound(_exinfo.userdata, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref _exinfo, out sound);

        if (playOnStart)
        {
            PlaySound(ref sound, ref channelGroup, ref channel);
        }
    }
    public void CreateSound(byte[] data,ref Sound sound, int numOfChannels, int sampleRate, ref CREATESOUNDEXINFO _exinfo, ChannelGroup channelGroup, Channel channel, bool playOnStart = true)
    {
        _exinfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
        _exinfo.numchannels = numOfChannels;
        _exinfo.format = FMOD.SOUND_FORMAT.PCM16;
        _exinfo.defaultfrequency = sampleRate;
        _exinfo.length = (uint)sampleRate * sizeof(short) * (uint)numOfChannels;
        RuntimeManager.CoreSystem.createSound(data, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref _exinfo, out sound);
    }
    public void PlaySound(ref Sound sound, ref ChannelGroup channelGroup, ref Channel channel)
    {
        RuntimeManager.CoreSystem.playSound(sound, channelGroup, true, out channel);
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