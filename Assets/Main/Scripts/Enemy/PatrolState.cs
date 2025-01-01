using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour
{
    [SerializeField] private float waypointThreshold = 1f; // ����������� ���������� �� ����� ��� � ����������
    private List<Transform> patrolWaypoints; // ������ ����� ��������������
    private int currentWaypointIndex = 0; // ������ ������� ����� �������
    [SerializeField]private EnemyMovement enemyMovement; // ������ �� ��������� �������� �����

    void Awake()
    {
        // �������� ������ �� ����������� ����������
        enemyMovement = GetComponent<EnemyMovement>();

        // �������� ��� ������� � ����� PatrolWaypoints
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("PatrolWaypoints");
        patrolWaypoints = new List<Transform>();
        foreach (GameObject waypoint in waypointObjects)
        {
            patrolWaypoints.Add(waypoint.transform);
        }

        if (patrolWaypoints.Count == 0)
        {
            Debug.LogWarning("��� ����� �������������� � ����� 'PatrolWaypoints'.");
        }
    }

    void OnEnable()
    {
        // ������������� ������ ����� �������
        if (patrolWaypoints.Count > 0)
        {
            SetNextWaypoint();
        }
    }

    void Update()
    {
        if (patrolWaypoints.Count == 0) return;

        // ���������, ������ �� ���� ������� �����
        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint <= waypointThreshold)
        {
            SetNextWaypoint();
        }
    }

    private void SetNextWaypoint()
    {
        // ������������� ��������� ����� �������
        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
        enemyMovement.SetTarget(patrolWaypoints[currentWaypointIndex].gameObject);
    }
}
