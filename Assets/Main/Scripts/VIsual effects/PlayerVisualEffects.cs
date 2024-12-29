using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerVisualEffects : MonoBehaviour
{
    private ObjectHealth health;
    private Volume volume;
    private Vignette vignette;         
    
    void Start()
    {
        health = GetComponent<ObjectHealth>();
        volume = FindObjectOfType<Volume>();

        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.Override(0f);
        }
        else
        {
            Debug.LogError("Vignette не найден в Volume.");
        }
    }

    void Update()
    {
        if (health != null && vignette != null)
        {
            float healthPercentage = health.currentHealth / health.maxHealth;

            UpdateVignetteIntensity(healthPercentage);
        }
    }

    private void UpdateVignetteIntensity(float healthPercentage)
    {

        vignette.intensity.Override(Mathf.Lerp(0.5f, 0f, healthPercentage));

    }
}
