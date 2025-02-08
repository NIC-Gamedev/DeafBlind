using UnityEngine;
using System.Collections;

public class UIHpBar : MonoBehaviour
{
    private ObjectHealth _objectHealth;
    private RectTransform _rectTransform;

    private float _fullWidth;    // �������� ������ ��� ������ �������� (��������, 270)
    private float _initialPosX;  // �������� ������� �� X (��������, 180)

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

        // �������� RectTransform ��� ��������� ������� UI ��������
        _rectTransform = GetComponent<RectTransform>();

        // ��������� �������� ������ � ������� ������
        _fullWidth = _rectTransform.sizeDelta.x;
        _initialPosX = _rectTransform.anchoredPosition.x;

        // ������������� �� ������� ��������� ��������
        ObjectHealth.OnHealthValueChange += OnHealthChanged;

       
    }

    void OnDestroy()
    {
        // ������������ �� �������, ����� �������� ������ ������
        ObjectHealth.OnHealthValueChange -= OnHealthChanged;
    }

    // �����, ���������� ��� ��������� ��������
    private void OnHealthChanged(float currentHealth)
    {
        // ��������� ������� ����������� ��������
        float healthPercentage = currentHealth / _objectHealth.maxHealth;
        // ��������� ����� ������ ������
        float newWidth = _fullWidth * healthPercentage;
        // ��������� ����� ������� �� ��� X:
        // ��� ������ �������� newWidth == _fullWidth, ������� �������� _initialPosX.
        // ��� ���������� ������ ������� ������� �� �������� �������.
        float newPosX = _initialPosX - (_fullWidth - newWidth) * 0.5f;

        // ���� ��� �������� ��������, ������������� �
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        // ��������� ��������, ������� ������ ������� ������ � �������
        currentAnimation = StartCoroutine(AnimateBar(newWidth, newPosX, animationDuration));
    }

    // �������� ��� ������� �������� ��������� ������ � �������
    private IEnumerator AnimateBar(float targetWidth, float targetPosX, float duration)
    {
        float elapsed = 0f;
        float startWidth = _rectTransform.sizeDelta.x;
        float startPosX = _rectTransform.anchoredPosition.x;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // ������������� ������ � ������� �� ��������� �� ������� ��������
            float width = Mathf.Lerp(startWidth, targetWidth, t);
            float posX = Mathf.Lerp(startPosX, targetPosX, t);

            // ��������� ������
            Vector2 newSizeDelta = _rectTransform.sizeDelta;
            newSizeDelta.x = width;
            _rectTransform.sizeDelta = newSizeDelta;

            // ��������� �������
            Vector2 newAnchoredPosition = _rectTransform.anchoredPosition;
            newAnchoredPosition.x = posX;
            _rectTransform.anchoredPosition = newAnchoredPosition;

            yield return null;
        }

        // ������������� ������ �������� ��������
        Vector2 finalSize = _rectTransform.sizeDelta;
        finalSize.x = targetWidth;
        _rectTransform.sizeDelta = finalSize;

        Vector2 finalAnchoredPosition = _rectTransform.anchoredPosition;
        finalAnchoredPosition.x = targetPosX;
        _rectTransform.anchoredPosition = finalAnchoredPosition;
    }
}
