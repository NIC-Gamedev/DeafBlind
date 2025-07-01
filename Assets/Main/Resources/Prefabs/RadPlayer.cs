using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using FishNet.Object;
using Unity.VisualScripting;
using FishNet.Component.Spawning;

public class RadiationManager : NetworkBehaviour
{
    [SerializeField]
    private FilmGrain grain;

    [SerializeField]
    private Volume postVolume;

    [SerializeField]
    private RadiationZone[] zones;

    
    private void GetRadZones()
    {
        zones = FindObjectsOfType<RadiationZone>();
        if (zones == null) Debug.Log("NoRadZones");
        
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            Debug.Log("NoWner");
            this.enabled = false;
            return;
        }
    }
    public void Start()
    {
           
        postVolume =GetComponentInChildren<Volume>();
        if (postVolume != null && postVolume.profile.TryGet(out FilmGrain g))
            grain = g;

        GetRadZones();
    }
    private void Update()
    {
        if (grain == null || zones == null)
            return;
        GetRadZones();
        float max = 0f;
        foreach (var zone in zones)
        {
            float intensity = zone.GetIntensity(transform.position);
            max = Mathf.Max(max, intensity);
        }

        grain.intensity.value = max;
    }
    
   
}
