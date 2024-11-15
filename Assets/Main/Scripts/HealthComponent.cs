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
        OnHealthValueChange?.Invoke(currentHealth);
    }

    public void AddHealth(float heal)
    {
        currentHealth += heal;
        OnHealthValueChange?.Invoke(currentHealth);
    }
}
