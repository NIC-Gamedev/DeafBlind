using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    private ObjectHealth _objectHealth;
    private Slider _slider;

    // �������� �����, ������� ����� ���������� ������ 5 ������ (��� �����)
    [SerializeField] private float testDamageAmount = 5f;
    // ������������ �������� ��������� ������ � ������� (� ��������)
    [SerializeField] private float animationDuration = 0.5f;

    // ������ �� ������� ������������ �������� (����� ��� ����� ��������� ���������� ����������)
    private Coroutine currentAnimation;

    void Start()
    {
        // �������� ��������� ObjectHealth �� ������������� �������
        _objectHealth = GetComponentInParent<ObjectHealth>();
        _slider = GetComponent<Slider>();
      

        // ������������� �� ������� ��������� ��������
        ObjectHealth.OnHealthValueChange += OnHealthChanged;

        StartCoroutine(TakeHP());
    }

    private IEnumerator TakeHP()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            _objectHealth.GetDamage(5f);
        }
    }

    void OnDestroy()
    {
        // ������������ �� �������, ����� �������� ������ ������
        ObjectHealth.OnHealthValueChange -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHealth)
    {
        float targetValue = currentHealth / _objectHealth.maxHealth * 100;

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(AnimateSlider(targetValue, animationDuration));
    }

    private IEnumerator AnimateSlider(float targetValue, float duration)
    {
        float elapsed = 0f;
        float startValue = _slider.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _slider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration) ;
            yield return null;
        }

        _slider.value = targetValue;
    }


}
