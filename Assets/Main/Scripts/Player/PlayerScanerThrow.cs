using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScanerThrow : WaveThrow
{
    [Header("StepSound")]

    [SerializeField] protected EventReference playerMoveSound;
    public float stepTime;
    protected float curentStepTime;
    [Range(0, 1)][SerializeField] protected float stepVol = 1;
    [Range(0, 1)][SerializeField] protected float stepPitch = 1;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMin = 0;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMax = 0;

    private Vector3 direction;

    private Vector3 colliderBottom => col.bounds.center - new Vector3(0, col.bounds.extents.y, 0);
    private Collider _col;
    private Collider col { get { 
            if (_col == null) 
            { 
                _col = GetComponent<Collider>();
            }
            return _col;
        } 
    }

    private PlayerNetworkMovement _playerMovement;
    private PlayerNetworkMovement playerMovement
    {
        get
        {
            if (_playerMovement == null)
            {
                _playerMovement = GetComponent<PlayerNetworkMovement>();
            }
            return _playerMovement;
        }
    }

    protected override void Start()
    {
        base.Start();
        InputManager.inputActions.Player.Movement.performed += context => direction = context.ReadValue<Vector3>();
        InputManager.inputActions.Player.Movement.canceled += context => direction = context.ReadValue<Vector3>();
        curentStepTime = stepTime;
    }
    protected void OnCollisionStay(Collision collision)
    {
        if (isMove() && curentStepTime < 0 && !playerMovement.isSneak)
        {
            curentStepTime = stepTime;
            Debug.Log("asdasd");
            if(!playerMovement.isSprinting)
                PhysicalAudioManager.instance.InstanceByPos(playerMoveSound, volume: 0.5f, minDistance: 0.1f, maxDistance: 5, pos: collision.contacts[0].point);
            else
                PhysicalAudioManager.instance.InstanceByPos(playerMoveSound, volume: 0.8f, minDistance: 0.1f, maxDistance: 8, pos: collision.contacts[0].point);
        }
        else
        {
            curentStepTime -= Time.deltaTime;
        }
    }

    public bool isMove()
    {
        if (direction.x != 0 || direction.z != 0)
        {
            return true;
        }
        return false;
    }
}
