using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputManagerEntry;

public class PlayerPause : MonoBehaviour
{
    MainController inputActions;
    public GameObject pausePanel;
    private PlayerNetworkMovement networkMovement;

    public void Start()
    {
        networkMovement = GetComponent<PlayerNetworkMovement>();
        InputInit();
    }


    private void InputInit()
    {
        inputActions.Player.Pause.Enable();
        inputActions.Player.Pause.performed += Pause;
    }

    private void Pause(InputAction.CallbackContext callback)
    {
        pausePanel.SetActive(true);
        networkMovement.enabled = false;
        
    }

    public void UnPause()
    {
        pausePanel.SetActive(false);
        networkMovement.enabled = true;
    }
}
