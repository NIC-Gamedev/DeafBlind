using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAudioListener : MonoBehaviour,IListenAudio
{
    public Transform lastHearAudio { get; set; }
    [SerializeField] private float _listenDistance;

    public float listenDistance
    {
        get => _listenDistance;
        set => _listenDistance = value;
    }

    public void OnListenAudio(Vector3 audioPos)
    {
        if (lastHearAudio != null)
            Destroy(lastHearAudio.gameObject);
        var lastHear = new GameObject("LastHearAudio").transform;
        lastHear.position = audioPos;
        lastHearAudio = lastHear;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 140, 0,255);

        Gizmos.DrawWireSphere(transform.position, listenDistance);
    }
}
