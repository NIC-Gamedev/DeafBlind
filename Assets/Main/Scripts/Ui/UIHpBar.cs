using UnityEngine;
using System.Collections;

public class UIHpBar : MonoBehaviour
{
    private ObjectHealth _objectHealth;
    private RectTransform _rectTransform;

    private float _fullWidth;    // Исходная ширина при полном здоровье (например, 270)
    private float _initialPosX;  // Исходная позиция по X (например, 180)

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

        // Получаем RectTransform для изменения размера UI элемента
        _rectTransform = GetComponent<RectTransform>();

        // Сохраняем исходную ширину и позицию панели
        _fullWidth = _rectTransform.sizeDelta.x;
        _initialPosX = _rectTransform.anchoredPosition.x;

        // Подписываемся на событие изменения здоровья
        ObjectHealth.OnHealthValueChange += OnHealthChanged;

       
    }

    void OnDestroy()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        ObjectHealth.OnHealthValueChange -= OnHealthChanged;
    }

    // Метод, вызываемый при изменении здоровья
    private void OnHealthChanged(float currentHealth)
    {
        // Вычисляем процент оставшегося здоровья
        float healthPercentage = currentHealth / _objectHealth.maxHealth;
        // Вычисляем новую ширину панели
        float newWidth = _fullWidth * healthPercentage;
        // Вычисляем новую позицию по оси X:
        // При полном здоровье newWidth == _fullWidth, позиция остается _initialPosX.
        // При уменьшении ширины смещаем позицию на половину разницы.
        float newPosX = _initialPosX - (_fullWidth - newWidth) * 0.5f;

        // Если уже запущена анимация, останавливаем её
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        // Запускаем корутину, которая плавно изменит ширину и позицию
        currentAnimation = StartCoroutine(AnimateBar(newWidth, newPosX, animationDuration));
    }

    // Корутина для плавной анимации изменения ширины и позиции
    private IEnumerator AnimateBar(float targetWidth, float targetPosX, float duration)
    {
        float elapsed = 0f;
        float startWidth = _rectTransform.sizeDelta.x;
        float startPosX = _rectTransform.anchoredPosition.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Интерполируем ширину и позицию от начальных до целевых значений
            float width = Mathf.Lerp(startWidth, targetWidth, t);
            float posX = Mathf.Lerp(startPosX, targetPosX, t);

            // Обновляем ширину
            Vector2 newSizeDelta = _rectTransform.sizeDelta;
            newSizeDelta.x = width;
            _rectTransform.sizeDelta = newSizeDelta;

            // Обновляем позицию
            Vector2 newAnchoredPosition = _rectTransform.anchoredPosition;
            newAnchoredPosition.x = posX;
            _rectTransform.anchoredPosition = newAnchoredPosition;

            yield return null;
        }

        // Устанавливаем точные конечные значения
        Vector2 finalSize = _rectTransform.sizeDelta;
        finalSize.x = targetWidth;
        _rectTransform.sizeDelta = finalSize;

        Vector2 finalAnchoredPosition = _rectTransform.anchoredPosition;
        finalAnchoredPosition.x = targetPosX;
        _rectTransform.anchoredPosition = finalAnchoredPosition;
    }
}
