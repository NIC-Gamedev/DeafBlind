using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour, IAIState
{
    [SerializeField] private float waypointThreshold = 1f; // ����������� ���������� �� �����
    private List<Transform> patrolWaypoints; // ������ ����� �������
    private int currentWaypointIndex = 0; // ������ ������� �����
    private EnemyMovement enemyMovement; // ������ �� EnemyMovement

    public void EnterState(GameObject owner)
    {
        enemyMovement = owner.GetComponent<EnemyMovement>();

        // �������� ����� �������
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
            Debug.LogWarning("��� ����� �������������� � ����� 'PatrolWaypoints'.");
        }
    }

    public void UpdateState()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count == 0) return;

        // ��������� ���������� �� ������� �����
        float distanceToWaypoint = Vector3.Distance(enemyMovement.transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint <= waypointThreshold)
        {
            SetNextWaypoint();
        }
    }

    public void ExitState()
    {
        // ����� �������� ������ �������, ���� ���������
    }

    private void SetNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
        enemyMovement.SetTarget(patrolWaypoints[currentWaypointIndex].gameObject);
    }
}
