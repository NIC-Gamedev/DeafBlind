<<<<<<< HEAD
using FishNet.Object;
=======
using AYellowpaper.SerializedCollections;
>>>>>>> dev/enemyAi
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseAnimator : NetworkBehaviour
{
    protected int CurrentState;
    protected float LockedTill;
    protected Animator anim;

    public bool isAnimReloaded;

    [SerializedDictionary] public SerializedDictionary<string,int> animationHash = new SerializedDictionary<string, int>();

    
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

    protected virtual void OnValidate()
    {
        if (!isAnimReloaded) 
        { 
            animationHash.Clear();
            InitAnimation();
            isAnimReloaded = true;
        }
    }

    protected virtual void InitAnimation()
    {

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
