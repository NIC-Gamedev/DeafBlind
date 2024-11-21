using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObjects : MonoBehaviour
{
    [SerializeField] private float interactRadius;
    [SerializeField] private UnityEvent OnInteractObject;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private bool useOnce;
    private bool isOn = false;

    private void Update()
    {
        Collider[] interactSphere = Physics.OverlapSphere(transform.position, interactRadius);
        foreach (Collider col in interactSphere)
        {
            if (col.TryGetComponent(out PlayerMovement playerMovement))
            {
                if (InputManager.inputActions.Player.Interact.WasPerformedThisFrame())
                {
                    if (useOnce)
                    {
                        if (!isOn)
                        {
                            OnInteractObject?.Invoke();
                            isOn = true;
                            AudioManager.instance.PlaySoundEffect(interactSound,maxDistance: 10 ,position: transform);
                        }
                    }
                    else
                    {
                        AudioManager.instance.PlaySoundEffect(interactSound, maxDistance: 10, position: transform);
                        if (isOn)
                        {
                            OnInteractObject?.Invoke();
                        }
                        else
                        {

                        }
                    }
                }
            }

            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
