using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth; 
    }

    public void GetDamage(float dmg)
    {
        currentHealth -= dmg;
    }

    public void AddHealth(float heal)
    {
        currentHealth += heal;      
    }
}
