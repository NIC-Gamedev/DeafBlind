using FishNet.Object;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
public abstract class BaseAnimator : NetworkBehaviour
{
    protected int CurrentState;
    protected float LockedTill;
    protected Animator anim;

    public bool isAnimReloaded;

    public Dictionary<string, int> animationHash = new Dictionary<string, int>();

    public List<AnimationClip> animationClips = new List<AnimationClip>();

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        InitAnimation();
        RuntimeAnimatorController controller = anim.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            animationClips.Add(clip);
        }
    }
    protected virtual void Update()
    {
        AnimationStateLogic();
    }

    private void AnimationStateLogic()
    {
        var state = GetState();

        if (state == CurrentState) return;
        //SetState(state);
        SendAnimationToServer(state);
    }

    protected virtual void InitAnimation(){}
    
    protected virtual int GetState()
    {
        return 0;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendAnimationToServer(int state)
    {
        if (state == CurrentState) return;

        SendAnimationToClients(state);
    }
    private void SetState(int state)
    {
        CurrentState = state;
        SendAnimationToClients(state);
    }
    [ObserversRpc]
    private void SendAnimationToClients(int state)
    {
        if (anim == null) return; 
        if (state == CurrentState) return;
        CurrentState = state;
        anim.CrossFade(state, 0.1f, 0);
    }

    public virtual int LockState(int s, float t)
    {
        LockedTill = t - Time.deltaTime;
        return s;
    }
}