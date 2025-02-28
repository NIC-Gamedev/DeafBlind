using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    [SerializeField] private ObjectHealth _objectHealth;
    private Slider _slider;
    
    [SerializeField] private float animationDuration = 0.5f;
    
    private Coroutine currentAnimation;

    void Start()
    {
        _slider = GetComponent<Slider>();
        
        ObjectHealth.OnHealthValueChange += OnHealthChanged;
    }

    void OnDestroy()
    {
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
