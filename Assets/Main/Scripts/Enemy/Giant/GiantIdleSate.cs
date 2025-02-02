using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantIdleState : MonoBehaviour, IAIState
{
    [SerializeField] private float idleTime;
    [SerializeField] private LayerMask detectLayer;
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

    private MapManager mapManager => ServiceLocator.instance.Get<MapManager>();

    public void EnterState(StateController owner)
    {
        timer = idleTime;
        controller = owner;
        enemyMovement.SetTarget(null);
        randomWayIndex = Random.Range(0, mapManager.mapData.allWayPoints.Count);
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
        if (timer < 0)
        {
            enemyMovement.SetTarget(ServiceLocator.instance.Get<MapManager>().mapData.allWayPoints[randomWayIndex]);
            controller.SetState<GiantPatrolState>();
        }

        if (enemyPerception.GetVisibleObjects().Count > 0)
        {
            controller.SetState<GiantChaseState>();
        }
    }
}
