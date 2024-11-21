using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjects : MonoBehaviour
{
    [SerializeField] private UnityEvent OnInteractObject;
    [SerializeField] private bool useOnce;
    protected bool isOn = false;

    public void InteractiveInvoke()
    {
        if (useOnce)
        {
            if (!isOn)
            {
                isOn = true;
                OnInteractObject?.Invoke();
            }
        }
        else
        {
            OnInteractObject?.Invoke();
        }
    }
}
