using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : BaseAnimator
{
    private MainController inputActions;
    private Vector3 input;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private List<ChainedAnimation> chainedAnimations = new List<ChainedAnimation>();
    private bool isJumpStart;

  

    protected override void Start()
    {
        base.Start();
        inputActions = InputManager.inputActions;
        inputActions.Player.Movement.performed += callback => input = callback.ReadValue<Vector3>();
        inputActions.Player.Movement.canceled += callback => input = callback.ReadValue<Vector3>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        InitAnimation();
    }

    private void InitAnimation()
    {
        animationHash.Add("Idle", Animator.StringToHash("Idle"));

        animationHash.Add("RunRight", Animator.StringToHash("HumanM@Run01_Right"));
        animationHash.Add("RunForwardRight", Animator.StringToHash("HumanM@Run01_ForwardRight"));
        animationHash.Add("RunForwardLeft", Animator.StringToHash("HumanM@Run01_ForwardLeft"));
        animationHash.Add("RunForward", Animator.StringToHash("HumanM@Run01_Forward"));
        animationHash.Add("RunLeft", Animator.StringToHash("HumanM@Run01_Left"));
        animationHash.Add("RunBackward", Animator.StringToHash("HumanM@Run01_Backward"));
        animationHash.Add("RunBackwardRight", Animator.StringToHash("HumanM@Run01_BackwardRight"));
        animationHash.Add("RunBackwardLeft", Animator.StringToHash("HumanM@Run01_BackwardLeft"));

        animationHash.Add("WalkRight", Animator.StringToHash("HumanM@Walk01_Right"));
        animationHash.Add("WalkForwardRight", Animator.StringToHash("HumanM@Walk01_ForwardRight"));
        animationHash.Add("WalkForwardLeft", Animator.StringToHash("HumanM@Walk01_ForwardLeft"));
        animationHash.Add("WalkForward", Animator.StringToHash("HumanM@Walk01_Forward"));
        animationHash.Add("WalkLeft", Animator.StringToHash("HumanM@Walk01_Left"));
        animationHash.Add("WalkBackward", Animator.StringToHash("HumanM@Walk01_Backward"));
        animationHash.Add("WalkBackwardRight", Animator.StringToHash("HumanM@Walk01_BackwardRight"));
        animationHash.Add("WalkBackwardLeft", Animator.StringToHash("HumanM@Walk01_BackwardLeft"));


        animationHash.Add("StartJump", Animator.StringToHash("HumanM@Jump01 - Start"));
        animationHash.Add("Fall", Animator.StringToHash("HumanM@Fall01"));
        animationHash.Add("Land", Animator.StringToHash("HumanM@Jump01 - Land"));
    }

    protected override int GetState()
    {
        bool isGround = playerMovement.IsOnGround();
        if (rb.linearVelocity.y > 0 && !isGround)
        {
            isJumpStart = true;
            return animationHash["StartJump"];
        }
        else if (rb.linearVelocity.y < 0 &&!isGround)
        {
            return animationHash["Fall"];
        }
        else if(isGround && isJumpStart)
        {
            isJumpStart = false;
            return animationHash["Land"];
        }

        if (input != Vector3.zero)
            return PlayerMoveAnimation(playerMovement.isSprinting);

        return animationHash["Idle"];
    }


    private int PlayerMoveAnimation(bool isSprint)
    {
        if (input == (Vector3.forward + Vector3.right))
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}ForwardRight"];
        }
        else if (input == Vector3.forward + Vector3.left)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}ForwardLeft"];
        }
        else if (input == Vector3.back + Vector3.right)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}BackwardRight"];
        }
        else if (input == Vector3.back + Vector3.left)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}BackwardLeft"];
        }
        else if (input == Vector3.forward)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}Forward"];
        }
        else if (input == Vector3.back)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}Backward"];
        }
        else if (input == Vector3.right)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}Right"];
        }
        else if (input == Vector3.left)
        {
            return animationHash[$"{(isSprint ? "Run" : "Walk")}Left"];
        }
        return animationHash["Idle"];
    }


    unsafe struct ChainedAnimation
    {
        float timeToNextAnim;
        string currentAnimationChain;
    }
}