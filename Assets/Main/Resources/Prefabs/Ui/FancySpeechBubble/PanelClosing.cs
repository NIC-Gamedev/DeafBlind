using System.Collections;
using UnityEngine;

public class PanelClosing : MonoBehaviour
{
    public float delay = 0.35f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DelayedPanelClose()
    {
        StartCoroutine(DelayPanelClose());
    }
    public IEnumerator DelayPanelClose()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // отключаем после завершения

    }
}
