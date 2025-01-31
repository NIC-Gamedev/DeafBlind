using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerAnimation : BaseAnimator
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
    public EnemyMeleeAttack meleAttack;
    public StalkerAttackState attackState;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (stateController == null)
        {
            stateController = GetComponent<StateController>();
        }
        if (meleAttack == null)
        {
            meleAttack = GetComponent<EnemyMeleeAttack>();
        }

        if (attackState == null)
        {
            attackState = GetComponent<StalkerAttackState>();
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
        animationHash.Add($"Run", Animator.StringToHash("Run"));
        animationHash.Add($"Attack", Animator.StringToHash("Attack"));
        animationHash.Add($"Back", Animator.StringToHash("Walk_Back"));
    }

    protected override int GetState()
    {

        if (meleAttack.attackProcess != null)
        {
            return animationHash["Attack"];
        }
        if (attackState.TryAttackProcces != null || stateController.currentState is StalkerHideState)
        {
            return animationHash[$"Run"];
        }
        if (stateController.currentState is StalkerTryToGoBehindState)
        {
            return animationHash[$"Back"];
        }
        if (stateController.currentState is StalkerPatrolState)
        {
            return animationHash[$"Walk"];
        }
        return animationHash["Idle"];
    }
}
