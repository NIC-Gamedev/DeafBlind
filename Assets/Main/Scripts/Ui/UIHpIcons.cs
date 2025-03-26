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
        _objectHealth.OnHealthValueChange += OnHealthChanged;
    }


    void OnDestroy()
    {
        _objectHealth.OnHealthValueChange -= OnHealthChanged;
    }

   
    private void OnHealthChanged(float currentHealth)
    {
        float healthPercentage = (currentHealth / _objectHealth.maxHealth) * 100f;

        if (healthPercentage >= 66.6f)
        {
            ActivateIcon(0); // ��� �������� 66.6% � ���� ������� ������ ������
        }
        else if (healthPercentage >= 33.33f)
        {
            ActivateIcon(1); // ��� �������� �� 33.33% �� 66.6% ������� ������ ������
        }
        else
        {
            ActivateIcon(2); // ��� �������� ���� 33.33% ������� ������ ������
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
