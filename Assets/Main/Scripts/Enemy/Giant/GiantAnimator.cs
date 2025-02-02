using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAnimator : BaseAnimator
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
    public GiantChaseState chaseState;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (stateController == null)
        {
            stateController = GetComponent<StateController>();
        }
        if (chaseState == null)
        {
            chaseState = GetComponent<GiantChaseState>();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void InitAnimation()
    {
        animationHash.Add("Idle", Animator.StringToHash("Idle"));

        animationHash.Add($"Run", Animator.StringToHash("Sprint"));
        animationHash.Add($"Walk", Animator.StringToHash("Walk"));
        animationHash.Add($"Attack", Animator.StringToHash("Attack"));
    }

    protected override int GetState()
    {
        if (stateController.currentState is GiantChaseState)
        {
            if (chaseState.reactionTime < 0)
            {
                return animationHash[$"Run"];
            }
        }

        if (stateController.currentState is GiantAttackState)
        {
            return animationHash["Attack"];
        }

        if (stateController.currentState is GiantPatrolState)
        {
            return animationHash[$"Walk"];
        }
        return animationHash["Idle"];
    }
}
