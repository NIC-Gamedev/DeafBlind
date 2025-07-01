using FishNet.Object;
using UnityEngine;

public class RadiationZone : MonoBehaviour
{
    [Range(1f, 100f)] public float maxDistance = 20f;
    [Range(0f, 1f)] public float maxIntensity = 1f;

    public float GetIntensity(Vector3 playerPosition)
    {
        float dist = Vector3.Distance(transform.position, playerPosition);
        float t = Mathf.Clamp01(1f - dist / maxDistance);
        return Mathf.Lerp(0f, maxIntensity, t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, maxDistance);
        Gizmos.color = new Color(1f, 0.4f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
