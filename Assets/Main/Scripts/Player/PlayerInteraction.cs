using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private MainController mainController;
    [SerializeField] private EventReference interactSound;
    [SerializeField] private EventReference cantInteractSound;
    [SerializeField] private float interactRadius;

    public void Start()
    {
        mainController = InputManager.inputActions;

        mainController.Player.Interact.Enable();
        mainController.Player.Interact.performed += Interact;
    }

    protected void Interact(InputAction.CallbackContext callback)
    {
        Collider[] interactSphere = Physics.OverlapSphere(transform.position, interactRadius);
        AudioManager.instance.Play(audioRef: cantInteractSound);
        for (int i = 0; i < interactSphere.Length; i++)
        {
            if (interactSphere[i].TryGetComponent(out InteractiveObjects interactiveObjects))
            {
                PhysicalAudioManager.instance.InstanceByTransform(audioRef: interactSound, transform: interactiveObjects.transform);
                interactiveObjects.InteractiveInvoke();
            }
        }
    }

    private void OnDisable()
    {
        mainController.Player.Interact.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
