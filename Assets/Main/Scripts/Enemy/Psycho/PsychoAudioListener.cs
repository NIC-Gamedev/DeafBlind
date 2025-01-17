using UnityEngine;

public class PsychoAudioListener : MonoBehaviour,IListenAudio
{
    public Transform lastHearAudio { get; set; }
    public void OnListenAudio(Vector3 audioPos) 
    {
        if(lastHearAudio != null)
            Destroy(lastHearAudio.gameObject);
        var lastHear = new GameObject("LastHearAudio").transform;
        lastHear.position = audioPos;
        lastHearAudio = lastHear;
    }
}

public interface IListenAudio
{
    public void OnListenAudio(Vector3 audioPos) { }
}
