using System;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    public Action<float> OnHealthValueChange;
    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthValueChange?.Invoke(currentHealth);
    }
    
    public void GetDamage(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthValueChange?.Invoke(currentHealth);
    }

    public void AddHealth(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthValueChange?.Invoke(currentHealth);
    }
}
