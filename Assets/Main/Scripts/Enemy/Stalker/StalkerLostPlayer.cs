using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerLostPlayer : MonoBehaviour,IAIState
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
    Transform latsSeenObject;


    StateController controller;
    public void EnterState(StateController owner)
    {
        controller = owner;
        if (enemyPerception.playerLastSeenPos != null)
        {
            if (latsSeenObject != null)
                Destroy(latsSeenObject.gameObject);

            latsSeenObject = new GameObject("lastSeenObjectTemp").transform;
            latsSeenObject.position = enemyPerception.playerLastSeenPos;
            enemyMovement.SetTarget(latsSeenObject);
            owner.SetState<StalkerPatrolState>();
        }
        else
        {
            owner.SetState<StalkerPatrolState>();
        }
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "LostPlayer";
    }

    public void UpdateState()
    {
    }
}
