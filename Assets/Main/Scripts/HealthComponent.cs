using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    public static Action<float> OnHealthValueChange;

    protected virtual void Start()
    {
        currentHealth = maxHealth; 
    }

    public void GetDamage(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0); // Убедиться, что здоровье не меньше 0
        OnHealthValueChange?.Invoke(currentHealth);
    }

    public void AddHealth(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Убедиться, что здоровье не больше MaxHealth
        OnHealthValueChange?.Invoke(currentHealth);
    }
}
