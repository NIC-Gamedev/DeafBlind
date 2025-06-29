using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class PlayerKnockedDown : MonoBehaviour
{
    [SerializeField] private float timeToDie;
    [SerializeField] private float timeToGetUp;
    private float _currTimeToDie;
    private float _currTimeToGetUp;
    public UnityEvent onKnockDown;
    public UnityEvent onGetUp;
    public UnityEvent onPlayerDeath;

    public Coroutine KnockDownProcess;
    public Coroutine GetUpProcess;

    public void OnKnockDown(float health)
    {
        if(health > 0)
            return;
        if (KnockDownProcess == null)
        {
            onKnockDown?.Invoke();
            KnockDownProcess = StartCoroutine(KnockDown());
        }
    }

    public void StartGetUp() 
    {
        if (GetUpProcess == null)
            GetUpProcess = StartCoroutine(GetUp());
    }
    public void StopGetUp() 
    {
        if (GetUpProcess != null)
            StopCoroutine(GetUpProcess);
    }

    private IEnumerator GetUp()
    {
        if (KnockDownProcess != null) //Shutdown knockdown coroutine
        {
            StopCoroutine(KnockDownProcess);
            KnockDownProcess = null;
        }
        while (_currTimeToGetUp > 0) //Start Timer
        {
            _currTimeToGetUp -= Time.deltaTime;
            yield return null;
        }
        onGetUp?.Invoke();
        GetUpProcess = null;
    }

    private IEnumerator KnockDown()
    {
        _currTimeToDie = timeToDie;
        while (_currTimeToDie > 0) //Start Timer
        {
            _currTimeToDie -= Time.deltaTime;
            yield return null;
        }
        onPlayerDeath?.Invoke();
    }
}

