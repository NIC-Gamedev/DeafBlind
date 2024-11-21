using UnityEngine;

public class JustMusicThrower : MonoBehaviour
{
    public AudioClip AudioClip;
    public Collider collider2;
    private void Start()
    {
        AudioManager.instance.PlayTrack(AudioClip,volumeCap:0.5f, collider:collider2,minDistance:2, maxDistance: 20);
    }
}
