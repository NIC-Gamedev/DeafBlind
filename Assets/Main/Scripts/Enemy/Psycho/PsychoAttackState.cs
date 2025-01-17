using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychoAttackState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    private EnemyPerception enemyPerception
    {
        get
        {
            if (_enemyPerception == null)
            {
                _enemyPerception = GetComponent<EnemyPerception>();
            }
            return _enemyPerception;
        }
    }
    private EnemyMovement _enemyMovement;
    private EnemyMovement enemyMovement
    {
        get
        {
            if (_enemyMovement == null)
            {
                _enemyMovement = GetComponent<EnemyMovement>();
            }
            return _enemyMovement;
        }
    }
    private PsychoAttack _psychoAttack;
    private PsychoAttack psychoAttack
    {
        get
        {
            if (_psychoAttack == null)
            {
                _psychoAttack = GetComponent<PsychoAttack>();
            }
            return _psychoAttack;
        }
    }


    StateController controller;
    public void EnterState(StateController owner)
    {
        controller = owner;
        psychoAttack.Attack();
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Attack";
    }

    public void UpdateState()
    {
        if(psychoAttack.attackProcess == null)
        {
            controller.SetState<PsychoChaseState>();
        }
    }
}
