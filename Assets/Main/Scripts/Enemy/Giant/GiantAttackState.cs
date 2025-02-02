using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAttackState : MonoBehaviour, IAIState
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
    private GiantAttack _AttackState;
    private GiantAttack attackState
    {
        get
        {
            if (_AttackState == null)
            {
                _AttackState = GetComponent<GiantAttack>();
            }
            return _AttackState;
        }
    }


    StateController controller;
    public void EnterState(StateController owner)
    {
        controller = owner;
        attackState.Attack();
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
        if (attackState.attackProcess == null)
        {
            controller.SetState<GiantChaseState>();
        }
    }
}
