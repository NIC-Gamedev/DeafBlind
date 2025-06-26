using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class PanelUnfold : MonoBehaviour
{
    [Header("Скорость анимации (сек)")]
    public float duration = 0.35f;

    private RectTransform rect;
    private float targetWidth;
    private Coroutine currentAnim;
    public List<GameObject> DelayOffList;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        targetWidth = rect.sizeDelta.x;
    }

    void OnEnable()
    {
        // Стартуем анимацию открытия
        rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
        StartExpand();
    }

    public void Close()
    {
        // Стартуем анимацию закрытия
        StartCollapse();
    }

  
    void StartExpand()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(Expand());
    }

    void StartCollapse()
    {
        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(Collapse());
    }

  
    IEnumerator Expand()
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float w = Mathf.Lerp(0, targetWidth, t / duration);
            rect.sizeDelta = new Vector2(w, rect.sizeDelta.y);
            yield return null;
        }
        rect.sizeDelta = new Vector2(targetWidth, rect.sizeDelta.y);
        foreach (GameObject obj in DelayOffList)
        {
            obj.SetActive(true);
        }
    }

    IEnumerator Collapse()
    {
        float startWidth = rect.sizeDelta.x;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float w = Mathf.Lerp(startWidth, 0, t / duration);
            rect.sizeDelta = new Vector2(w, rect.sizeDelta.y);
            yield return null;
        }
        rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
    }
}
