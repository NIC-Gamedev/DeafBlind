using System.Collections.Generic;
using UnityEngine;

public class PsychoAnimation : BaseAnimator
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
    public PsychoChaseState chaseState;

    private void OnValidate()
    {
        if (stateController == null)
        {
            stateController = GetComponent<StateController>();
        }
        if (chaseState == null)
        {
            chaseState = GetComponent<PsychoChaseState>();
        }
    }

    protected override void Start()
    {
        base.Start();
        InitAnimation();
    }

    private void InitAnimation()
    {
        animationHash.Add("Idle", Animator.StringToHash("Idle"));

        animationHash.Add($"Run", Animator.StringToHash("HumanM@Run01_Forward"));
        animationHash.Add($"Walk", Animator.StringToHash("HumanM@Walk01_Forward"));
    }

    protected override int GetState()
    {
        if (stateController.currentState is PsychoChaseState)
        {
            if (chaseState.isPlayeLook)
                anim.speed = 0;
            else
                anim.speed = 1;
            if (chaseState.reactionTime < 0)
            {
                return animationHash[$"Run"];
            }
        }
        else
            anim.speed = 1;

        if (stateController.currentState is PsychoPatrolState)
        {
            return animationHash[$"Walk"];
        }
        return animationHash["Idle"];
    }
}