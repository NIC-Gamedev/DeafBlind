using UnityEngine;

public class PsychoAudioListener : BaseAudioListener
{
}

public interface IListenAudio
{
    public float listenDistance { get; set; }
    public void OnListenAudio(Vector3 audioPos) { }
}
