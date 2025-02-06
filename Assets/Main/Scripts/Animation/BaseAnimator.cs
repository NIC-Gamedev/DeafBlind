using AYellowpaper.SerializedCollections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseAnimator : NetworkBehaviour
{
    protected int CurrentState;
    protected float LockedTill;
    protected Animator anim;

    public bool isAnimReloaded;

    [SerializedDictionary] public SerializedDictionary<string, int> animationHash = new SerializedDictionary<string, int>();

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }
    protected virtual void Update()
    {
        if(base.IsOwner)
            PlayerAnimationStateLogic();
    }

    protected void PlayerAnimationStateLogic()
    {
        var state = GetState();

        if (state == CurrentState) return;

        SendAnimationToServer(state);
    }

    protected override void OnValidate()
    {
        base.OnValidate();

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
    }

    [ServerRpc]
    private void SendAnimationToServer(int state)
    {
        if (state == CurrentState) return;

        CurrentState = state;
        SendAnimationToClients(state);
    }
    [ObserversRpc]
    private void SendAnimationToClients(int state)
    {
        anim.CrossFade(state, 0.1f, 0);
    }

    public virtual int LockState(int s, float t)
    {
        LockedTill = Time.time + t;
        return s;
    }
}