using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoice : MonoBehaviour
{
    public bool useMic;

    private void Start()
    {
        if (useMic)
        {
            if(Microphone.devices.Length > 0)
            {
                var audioTrack = AudioManager.instance.PlaySoundEffect(Microphone.Start(Microphone.devices[0].ToString(),true,1,AudioSettings.outputSampleRate),loop:true,minDistance:5,maxDistance:10,position:transform);
                audioTrack.transform.position = transform.position;
                audioTrack.transform.SetParent(transform);
            }
            else
            {
                useMic = false;
            }
        }
    }
}
