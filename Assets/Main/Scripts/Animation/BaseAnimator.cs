using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseAnimator : MonoBehaviour
{
    protected int CurrentState;
    protected float LockedTill;
    protected Animator anim;

    protected Dictionary<object,int> animationHash = new Dictionary<object, int>();

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }
    protected virtual void Update()
    {
        PlayerAnimationStateLogic();
    }
    protected void PlayerAnimationStateLogic()
    {
        var state = GetState();

        if (state == CurrentState) return;
        anim.CrossFade(state, 0.1f, 0);
        CurrentState = state;
    }

    protected virtual int GetState()
    {
        return 0;     

        int LockState(int s, float t)
        {
            LockedTill = Time.time + t;
            return s;
        }
    }
}
