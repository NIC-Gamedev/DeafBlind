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

        animationHash.Add($"Run{Vector3.right}", Animator.StringToHash("HumanM@Run01_Right"));
        animationHash.Add($"Run{Vector3.forward + Vector3.right}", Animator.StringToHash("HumanM@Run01_ForwardRight"));
        animationHash.Add($"Run{Vector3.forward + Vector3.left}", Animator.StringToHash("HumanM@Run01_ForwardLeft"));
        animationHash.Add($"Run{Vector3.forward}", Animator.StringToHash("HumanM@Run01_Forward"));
        animationHash.Add($"Run{Vector3.left}", Animator.StringToHash("HumanM@Run01_Left"));
        animationHash.Add($"Run{Vector3.back}", Animator.StringToHash("HumanM@Run01_Backward"));
        animationHash.Add($"Run{Vector3.back + Vector3.right}", Animator.StringToHash("HumanM@Run01_BackwardRight"));
        animationHash.Add($"Run{Vector3.back + Vector3.left}", Animator.StringToHash("HumanM@Run01_BackwardLeft"));
                          
        animationHash.Add($"Walk{Vector3.right}", Animator.StringToHash("HumanM@Walk01_Right"));
        animationHash.Add($"Walk{Vector3.forward + Vector3.right}", Animator.StringToHash("HumanM@Walk01_ForwardRight"));
        animationHash.Add($"Walk{Vector3.forward + Vector3.left}", Animator.StringToHash("HumanM@Walk01_ForwardLeft"));
        animationHash.Add($"Walk{Vector3.forward}", Animator.StringToHash("HumanM@Walk01_Forward"));
        animationHash.Add($"Walk{Vector3.left}", Animator.StringToHash("HumanM@Walk01_Left"));
        animationHash.Add($"Walk{Vector3.back}", Animator.StringToHash("HumanM@Walk01_Backward"));
        animationHash.Add($"Walk{Vector3.back + Vector3.right}", Animator.StringToHash("HumanM@Walk01_BackwardRight"));
        animationHash.Add($"Walk{Vector3.back + Vector3.left}", Animator.StringToHash("HumanM@Walk01_BackwardLeft"));


        animationHash.Add("StartJump", Animator.StringToHash("HumanM@Jump01 - Start"));
        animationHash.Add("Fall", Animator.StringToHash("HumanM@Fall01"));
        animationHash.Add("Land", Animator.StringToHash("HumanM@Jump01 - Land"));
    }

    protected override int GetState()
    {
        bool isGround = playerMovement.IsOnGround();
        if (rb.velocity.y > 0 && !isGround)
        {
            isJumpStart = true;
            return animationHash["StartJump"];
        }
        else if (rb.velocity.y < 0 &&!isGround)
        {
            return animationHash["Fall"];
        }
        else if(isGround && isJumpStart)
        {
            isJumpStart = false;
            return animationHash["Land"];
        }

        if (input != Vector3.zero)
            return PlayerMoveAnimation(playerMovement.isSprinting,playerMovement.isSneak);

        return animationHash["Idle"];
    }


    private int PlayerMoveAnimation(bool isSprint,bool isSit)
    {
        return animationHash[$"{(isSprint ? "Run" : "Walk")}{input}"];
    }


    unsafe struct ChainedAnimation
    {
        float timeToNextAnim;
        string currentAnimationChain;
    }
}