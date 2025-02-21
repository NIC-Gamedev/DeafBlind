using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        NavMeshHit hit;
        if (NavMesh.SamplePosition(audioPos, out hit, 1.0f, NavMesh.AllAreas))
        {
            lastHear.position = new Vector3(hit.position.x, audioPos.y, hit.position.z);
        }
        else
        {
            if (NavMesh.SamplePosition(audioPos, out hit, 10.0f, NavMesh.AllAreas))
            {
                lastHear.position = new Vector3(hit.position.x, audioPos.y, hit.position.z);
            }
            else
            {
                lastHear.position = audioPos;
            }
        }
        lastHearAudio = lastHear;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 140, 0,255);

        Gizmos.DrawWireSphere(transform.position, listenDistance);
    }
}
