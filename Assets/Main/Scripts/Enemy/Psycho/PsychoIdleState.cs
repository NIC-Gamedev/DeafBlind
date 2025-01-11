using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychoIdleState : MonoBehaviour,IAIState
{
    [SerializeField]private float idleTime;
    [SerializeField]private LayerMask detectLayer;
    private float timer;
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

    private StateController controller;

    private int randomWayIndex;

    private TestCheckpointManager checkpointManager => TestCheckpointManager.instance;

    public void EnterState(StateController owner)
    {
        enemyMovement.SetTarget(null);
        timer = idleTime;
        controller = owner;
        randomWayIndex = Random.Range(0, checkpointManager.points.Length);
    }
    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Idle";
    }

    public void UpdateState()
    {
        timer -= Time.deltaTime;
        if( timer < 0)
        {
            enemyMovement.SetTarget(checkpointManager.points[randomWayIndex]);
            controller.SetState<PsychoPatrolState>();
        }

        if (enemyPerception.GetVisibleObjects().Count > 0)
        {
            controller.SetState<PsychoChaseState>();
        }
    }
}
