using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyAnimator : BaseAnimator
{
    private Rigidbody _rb;
    private Rigidbody rb
    {
        get
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
        set
        { }
    }

    public StateController stateController;
    public FireFlyChaseState chaseState;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (stateController == null)
        {
            stateController = GetComponent<StateController>();
        }
        if (chaseState == null)
        {
            chaseState = GetComponent<FireFlyChaseState>();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void InitAnimation()
    {
        animationHash.Add("Idle", Animator.StringToHash("Idle"));

        animationHash.Add($"Walk", Animator.StringToHash("Walk"));
        animationHash.Add($"Attack", Animator.StringToHash("Attack"));
    }

    protected override int GetState()
    {

        if (stateController.currentState is FireFlyAttackState)
        {
            return animationHash["Attack"];
        }

        if (stateController.currentState is FireFlyPatrolState || stateController.currentState is FireFlyChaseState)
        {
            return animationHash[$"Walk"];
        }
        return animationHash["Idle"];
    }
}
