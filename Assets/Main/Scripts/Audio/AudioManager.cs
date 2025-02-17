using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Play(EventReference audioRef)
    {
        Play(audioRef,1,1,false);
    }

    public EventInstance Play(EventReference audioRef, float volume = 1, float pitch = 1, bool loop = false)
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
    public void StopTrack(EventReference reference)
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