using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour, IAIState
{
    [SerializeField] private float waypointThreshold = 1f; // Минимальное расстояние до точки
    private List<Transform> patrolWaypoints; // Список точек патруля
    private int currentWaypointIndex = 0; // Индекс текущей точки
    private EnemyMovement enemyMovement; // Ссылка на EnemyMovement

    public void EnterState(GameObject owner)
    {
        enemyMovement = owner.GetComponent<EnemyMovement>();

        // Собираем точки патруля
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("PatrolWaypoints");
        patrolWaypoints = new List<Transform>();
        foreach (GameObject waypoint in waypointObjects)
        {
            patrolWaypoints.Add(waypoint.transform);
        }

        if (patrolWaypoints.Count > 0)
        {
            SetNextWaypoint();
        }
        else
        {
            Debug.LogWarning("Нет точек патрулирования с тегом 'PatrolWaypoints'.");
        }
    }

    public void UpdateState()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

        // Проверяем расстояние до текущей точки
        float distanceToWaypoint = Vector3.Distance(enemyMovement.transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint <= waypointThreshold)
        {
            SetNextWaypoint();
        }
    }

    public void ExitState()
    {
        // Можно добавить логику очистки, если требуется
    }

    private void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
        enemyMovement.SetTarget(patrolWaypoints[currentWaypointIndex].gameObject);
    }
}
