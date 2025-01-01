using Unity.VisualScripting;
using UnityEngine;

public class PlayerScanerThrow : WaveThrow
{
    [Header("StepSound")]

    [SerializeField] protected AudioClip playerMoveSound;
    public float stepTime;
    protected float curentStepTime;
    [Range(0, 1)][SerializeField] protected float stepVol = 1;
    [Range(0, 1)][SerializeField] protected float stepPitch = 1;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMin = 0;
    [Range(0, 1)][SerializeField] protected float stepPitchRandomMax = 0;

    private Vector3 direction;
    protected override void Start()
    {
        base.Start();
        InputManager.inputActions.Player.Movement.performed += context => direction = context.ReadValue<Vector3>();
        InputManager.inputActions.Player.Movement.canceled += context => direction = context.ReadValue<Vector3>();
        curentStepTime = stepTime;
    }
    protected void OnCollisionStay(Collision collision)
    {
        curentStepTime -= Time.deltaTime;  
        if (isMove() && curentStepTime < 0)
        {
            curentStepTime = stepTime;
            audioManager.PlaySoundEffect(playerMoveSound, volume: 0.5f,minDistance:0.1f,maxDistance:5, ColideObject: collision);
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
