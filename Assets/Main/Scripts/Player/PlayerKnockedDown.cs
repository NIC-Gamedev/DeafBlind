using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class PlayerKnockedDown : MonoBehaviour
{
    [SerializeField] private float timeToGetUp;
    private float _currTimeToGetUp;
    public UnityEvent onKnockDown;
    public UnityEvent onGetUp;

    public bool isKnockDown;
    public Coroutine GetUpProcess;

    public void OnKnockDown(float health)
    {
        if(health > 0)
            return;
        if (!isKnockDown)
        {
            isKnockDown = true;
            onKnockDown?.Invoke();
        }
    }

    public void StartGetUp() 
    {
        if (isKnockDown)
        {
            if (GetUpProcess == null)
                GetUpProcess = StartCoroutine(GetUp());   
        }
    }
    public void StopGetUp() 
    {
        if (isKnockDown)
        {
            Debug.Log("Stop Get up");
            if (GetUpProcess != null)
            {
                StopCoroutine(GetUpProcess);
                GetUpProcess = null;
            }
        }
    }

    private IEnumerator GetUp()
    {
        _currTimeToGetUp = timeToGetUp;
        while (_currTimeToGetUp > 0) //Start Timer
        {
            _currTimeToGetUp -= Time.deltaTime;
            Debug.Log("Get up Processing");
            yield return null;
        }
        onGetUp?.Invoke();
        isKnockDown = false;
        GetUpProcess = null;
    }
}

