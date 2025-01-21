using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAudioListener : MonoBehaviour,IListenAudio
{
    public Transform lastHearAudio { get; set; }
    public void OnListenAudio(Vector3 audioPos)
    {
        if (lastHearAudio != null)
            Destroy(lastHearAudio.gameObject);
        var lastHear = new GameObject("LastHearAudio").transform;
        lastHear.position = audioPos;
        lastHearAudio = lastHear;
    }
}
