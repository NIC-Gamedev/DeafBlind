using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour
{
    [SerializeField] private float waypointThreshold = 1f; // Минимальное расстояние до точки для её достижения
    private List<Transform> patrolWaypoints; // Список точек патрулирования
    private int currentWaypointIndex = 0; // Индекс текущей точки патруля
    [SerializeField]private EnemyMovement enemyMovement; // Ссылка на компонент движения врага

    void Awake()
    {
        // Получаем ссылки на необходимые компоненты
        enemyMovement = GetComponent<EnemyMovement>();

        // Собираем все объекты с тегом PatrolWaypoints
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("PatrolWaypoints");
        patrolWaypoints = new List<Transform>();
        foreach (GameObject waypoint in waypointObjects)
        {
            patrolWaypoints.Add(waypoint.transform);
        }

        if (patrolWaypoints.Count == 0)
        {
            Debug.LogWarning("Нет точек патрулирования с тегом 'PatrolWaypoints'.");
        }
    }

    void OnEnable()
    {
        // Устанавливаем первую точку патруля
        if (patrolWaypoints.Count > 0)
        {
            SetNextWaypoint();
        }
    }

    void Update()
    {
        if (patrolWaypoints.Count == 0) return;

        // Проверяем, достиг ли враг текущей точки
        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint <= waypointThreshold)
        {
            SetNextWaypoint();
        }
    }

    private void SetNextWaypoint()
    {
        // Устанавливаем следующую точку патруля
        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
        enemyMovement.SetTarget(patrolWaypoints[currentWaypointIndex].gameObject);
    }
}
