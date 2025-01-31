using System;
using System.Collections;
using UnityEngine;

public class StalkerRevealState : MonoBehaviour, IAIState
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

    private StalkerStalking _stalking;
    private StalkerStalking stalking
    {
        get
        {
            if (_stalking == null)
            {
                _stalking = GetComponent<StalkerStalking>();
            }
            return _stalking;
        }
    }

    StateController controller;
    public void EnterState(StateController owner)
    {
        controller = owner; 
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Reveal";
    }

    public void UpdateState()
    {
        if (enemyPerception.IsPlayerLookingAtMe(stalking.huntingPlayer.transform))
        {
            stalking.timerBefAttack -= Time.deltaTime;


            if (stalking.timerBefAttack < 0)
            {
                controller.SetState<StalkerAttackState>();
            }
        }
        else
        {
            controller.SetState<StalkerHideState>();
        }
    }
}