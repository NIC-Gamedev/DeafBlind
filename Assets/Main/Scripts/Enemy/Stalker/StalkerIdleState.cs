using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class StalkerIdleState : NetworkBehaviour ,IAIState
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
        if (IsServer)
        {
            int newIndex = Random.Range(0, mapManager.mapData.allWayPoints.Count);
            SetWaypoint(newIndex);
        }
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
            controller.SetState<StalkerPatrolState>();
        }

        if (enemyPerception.GetVisibleObjects().Count > 0)
        {
            controller.SetState<StalkerHuntState>();
        }
    }
    
    [ObserversRpc]
    private void SetWaypoint(int index)
    {
        randomWayIndex = index;
    }
}
