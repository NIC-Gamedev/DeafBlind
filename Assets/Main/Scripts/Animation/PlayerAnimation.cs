using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : BaseAnimator
{
    private PlayerNetworkMovement _playerMovement;
    private PlayerNetworkMovement PlayerMovement {
        get 
        { 
            if (_playerMovement == null) 
            {
                TryGetComponent(out _playerMovement);
            }
            return _playerMovement;
        }
    }
    private HandHolder _handHolder;
    private HandHolder HandHolder {
        get 
        { 
            if (_handHolder == null) 
            {
                TryGetComponent(out _handHolder);
            }
            return _handHolder;
        }
    }
    private Rigidbody _rb; 
    private Rigidbody Rb 
    {
        get
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
        }
    }
    
    private PlayerKnockedDown _knockDown; 
    private PlayerKnockedDown KnockDown
    {
        get
        {
            if (_knockDown == null)
            {
                TryGetComponent(out _knockDown);
            }
            return _knockDown;
        }
    }
    private bool isJumpStart;
    

    protected override void InitAnimation()
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

        animationHash.Add($"CrouchIdle", Animator.StringToHash("Human@Crouch01_Idle"));
        animationHash.Add($"Crouch{Vector3.right}", Animator.StringToHash("Human@Crouch01_Walk_Right"));
        animationHash.Add($"Crouch{Vector3.forward + Vector3.right}", Animator.StringToHash("Human@Crouch01_Walk_ForwardRight"));
        animationHash.Add($"Crouch{Vector3.forward + Vector3.left}", Animator.StringToHash("Human@Crouch01_Walk_ForwardLeft"));
        animationHash.Add($"Crouch{Vector3.forward}", Animator.StringToHash("Human@Crouch01_Walk_Forward"));
        animationHash.Add($"Crouch{Vector3.left}", Animator.StringToHash("Human@Crouch01_Walk_Left"));
        animationHash.Add($"Crouch{Vector3.back}", Animator.StringToHash("Human@Crouch01_Walk_Backward"));
        animationHash.Add($"Crouch{Vector3.back + Vector3.right}", Animator.StringToHash("Human@Crouch01_Walk_BackwardRight"));
        animationHash.Add($"Crouch{Vector3.back + Vector3.left}", Animator.StringToHash("Human@Crouch01_Walk_BackwardLeft"));


        animationHash.Add("StartJump", Animator.StringToHash("HumanM@Jump01 - Start"));
        animationHash.Add("Fall", Animator.StringToHash("HumanM@Fall01"));
        animationHash.Add("Land", Animator.StringToHash("HumanM@Jump01 - Land"));
        
        animationHash.Add("Drop", Animator.StringToHash("mixamo_com"));
        animationHash.Add("Attack", Animator.StringToHash("2H@Attack02"));
        animationHash.Add("Die", Animator.StringToHash("Death"));
    }
    
    protected override int GetState()
    {
        bool isGround = PlayerMovement.IsOnGround();

        if (KnockDown != null)
        {
            if (KnockDown.isKnockDown)
            {
                return animationHash["Die"];
            }
        }
        
        if (HandHolder.currentItemObject != null)
        {
            if (HandHolder.currentItemObject.TryGetComponent(out UsablePipe usablePipe))
            {
                if (usablePipe.attackProcess != null)
                {
                    return animationHash["Attack"];
                }
            }
        }
        
        if (HandHolder.isDropping)
        {
            return animationHash["Drop"];
        }


        if (HandHolder.isDropping)
        {
            return animationHash["Drop"];
        }

        if (Rb.linearVelocity.y > 0 && !isGround)
        {
            isJumpStart = true;
            return animationHash["StartJump"];
        }
        else if (Rb.linearVelocity.y < 0 &&!isGround)
        {
            return animationHash["Fall"];
        }
        else if(isGround && isJumpStart)
        {
            isJumpStart = false;
            return animationHash["Land"];
        }

        if (PlayerMovement.input != Vector3.zero && (Mathf.Abs(Rb.linearVelocity.z) > 0.5 || Mathf.Abs(Rb.linearVelocity.x) > 0.5) )
            return PlayerMoveAnimation(PlayerMovement.isSprinting,PlayerMovement.isSneak);

        return PlayerMovement.isSneak == false ? animationHash["Idle"] : animationHash["CrouchIdle"];
    }


    private int PlayerMoveAnimation(bool isSprint,bool isSit)
    {
        if (isSit)
            return animationHash[$"Crouch{PlayerMovement.input}"];

        return animationHash[$"{(isSprint ? "Run" : "Walk")}{PlayerMovement.input}"];
    }
}