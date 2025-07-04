using System.Collections.Generic;
using FishNet.Object;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : NetworkBehaviour
{
    private MainController mainController;
    [SerializeField] private EventReference interactSound;
    [SerializeField] private EventReference cantInteractSound;
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactableLayer;
    private Collider[] interactSphere;
    private List<IInteractable> _interactables = new List<IInteractable>();

    public override void OnStartClient()
    {
        base.OnStartClient();
            
        if (!IsOwner)
        {
            return;
        }
        mainController = InputManager.inputActions;

        mainController.Player.Interact.Enable();
        mainController.Player.Interact.performed += Interact;
        mainController.Player.Interact.canceled += UnInteract;
    }

    protected void Interact(InputAction.CallbackContext callback)
    {
        Debug.Log("INTERACCT");
        interactSphere = Physics.OverlapSphere(transform.position, interactRadius,interactableLayer);
        _interactables.Clear();
        if(interactSphere.Length == 0)
            AudioManager.instance.Play(audioRef: cantInteractSound);
        for (int i = 0; i < interactSphere.Length; i++)
        {
            if (interactSphere[i].TryGetComponent(out IInteractable interactiveObjects))
            {
                if(interactSphere[i].gameObject == gameObject)
                    continue;
                PhysicalAudioManager.instance.InstanceByTransform(audioRef: interactSound, transform: interactSphere[i].transform);
                interactiveObjects.OnInteract();
                _interactables.Add(interactiveObjects);
            }
        }
    }
    protected void UnInteract(InputAction.CallbackContext callback)
    {
        Debug.Log("UnInteract");
        foreach (var interactable in _interactables)
        {
            Debug.Log(interactable.GetType());
            interactable.OnUnInteract();
        }
    }

    private void OnDisable()
    {
        if (!IsOwner)
        {
            return;
        }
        mainController.Player.Interact.Disable();
    }
    private void OnEnable()
    {
        if (!IsOwner)
        {
            return;
        }
        mainController.Player.Interact.Enable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
