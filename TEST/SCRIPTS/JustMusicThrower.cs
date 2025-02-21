using UnityEngine;

public class JustMusicThrower : MonoBehaviour
{
    public AudioClip AudioClip;
    public Collider collider2;
    private void Start()
    {
        PhysicalAudioManager.instance.PlayMusic(AudioClip, volume: 0.5f, pos:transform,minDistance:2, maxDistance: 20);
    }
}
