using System;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;
    public Action<float> OnHealthValueChange; 
    public Action knockOut;
    protected virtual void Start()
    {
        currentHealth = maxHealth; 
    }
    public void GetDamage(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Max(currentHealth, 0); // ���������, ��� �������� �� ������ 0
        OnHealthValueChange?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            knockOut?.Invoke();
        }
    }

    public void AddHealth(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // ���������, ��� �������� �� ������ MaxHealth
        OnHealthValueChange?.Invoke(currentHealth);
    }
}
