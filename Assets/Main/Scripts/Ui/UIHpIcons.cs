using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UIHpIcons : MonoBehaviour
{
    private ObjectHealth _objectHealth;
    private RectTransform _rectTransform;

    [SerializeField]
    private List<GameObject> _iconList = new List<GameObject>();

    
    void Start()
    {
        _objectHealth = GetComponentInParent<ObjectHealth>();
        ObjectHealth.OnHealthValueChange += OnHealthChanged;
    }


    void OnDestroy()
    {
        ObjectHealth.OnHealthValueChange -= OnHealthChanged;
    }

   
    private void OnHealthChanged(float currentHealth)
    {
        float healthPercentage = (currentHealth / _objectHealth.maxHealth) * 100f;

        if (healthPercentage >= 66.6f)
        {
            ActivateIcon(0); // ѕри здоровье 66.6% и выше активна перва€ иконка
        }
        else if (healthPercentage >= 33.33f)
        {
            ActivateIcon(1); // ѕри здоровье от 33.33% до 66.6% активна втора€ иконка
        }
        else
        {
            ActivateIcon(2); // ѕри здоровье ниже 33.33% активна треть€ иконка
        }
    }

    private void ActivateIcon(int activeIndex)
    {
        for (int i = 0; i < _iconList.Count; i++)
        {
            if (_iconList[i] != null)
            {
                _iconList[i].SetActive(i == activeIndex);
            }
        }
    }
}
