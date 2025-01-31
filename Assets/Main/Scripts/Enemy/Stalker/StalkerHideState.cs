using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StalkerHideState : MonoBehaviour,IAIState
{
    private int randomWayIndex;
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

    private MapManager mapManager => ServiceLocator.instance.Get<MapManager>();
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
        enemyMovement.movementSpeedMultiplier = 4f;
        stalking.stalkingLevel = stalking._stalkingLevel;
        SelectWaypoint(transform.position, stalking.huntingPlayer.transform.position,15,40);
    }

    public void UpdateState()
    {
        if(Vector3.Distance(transform.position, enemyMovement.targetPosition) < 1)
        {
            enemyMovement.movementSpeedMultiplier = 1f;
            controller.SetState<StalkerIdleState>();
        }
    }

    public string GetStateName()
    {
        return "StalkerHideState";
    }

    public void ExitState()
    {
        stalking.huntingPlayer = null;
    }

    public void SelectWaypoint(Vector3 enemyPosition, Vector3 playerPosition, float minDistanceFromPlayer, float maxDistanceFromEnemy)
    {
        // Получаем все вейпоинты
        var waypoints = ServiceLocator.instance.Get<MapManager>().mapData.allWayPoints;

        // Фильтруем вейпоинты по условиям
        var filteredWaypoints = waypoints
            .Where(waypoint => Vector3.Distance(waypoint.position, playerPosition) >= minDistanceFromPlayer
                               && Vector3.Distance(waypoint.position, enemyPosition) <= maxDistanceFromEnemy)
            .ToList();

        // Если есть подходящие вейпоинты
        if (filteredWaypoints.Count > 0)
        {
            // Выбираем случайный из подходящих
            int randomIndex = Random.Range(0, filteredWaypoints.Count);
            enemyMovement.SetTarget(filteredWaypoints[randomIndex]);
        }
        else
        {
            // Нет подходящих вейпоинтов, можно задать fallback-логику
            Debug.LogWarning("Нет подходящих вейпоинтов. Используется случайный.");
            int randomIndex = Random.Range(0, waypoints.Count);
            enemyMovement.SetTarget(waypoints[randomIndex]);
        }
    }
}
