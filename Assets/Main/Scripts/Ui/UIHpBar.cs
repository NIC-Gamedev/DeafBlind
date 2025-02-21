using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    private ObjectHealth _objectHealth;
    private Slider _slider;

    // Значение урона, которое будет наноситься каждые 5 секунд (для теста)
    [SerializeField] private float testDamageAmount = 5f;
    // Длительность анимации изменения ширины и позиции (в секундах)
    [SerializeField] private float animationDuration = 0.5f;

    // Ссылка на текущую анимационную корутину (чтобы при новом изменении остановить предыдущую)
    private Coroutine currentAnimation;

    void Start()
    {
        // Получаем компонент ObjectHealth из родительского объекта
        _objectHealth = GetComponentInParent<ObjectHealth>();
        _slider = GetComponent<Slider>();
      

        // Подписываемся на событие изменения здоровья
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
        // Отписываемся от события, чтобы избежать утечек памяти
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
