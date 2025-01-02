using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f; // ������ �����������
    [SerializeField] private float fieldOfViewAngle = 360f; // ���� ���� ������
    [SerializeField] private LayerMask targetLayer; // ����� ���� ��� ����� (��������, �����)

    private StateController stateController; // ������ �� ���������� ���������
    private Transform player; // ������ �� ������
    private bool playerInSight = false; // ����, ������������, ��� ����� � ���� ������

    private void Awake()
    {
        stateController = GetComponent<StateController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // ���������, ����� �� ���� ������
        CheckPlayerSight();

        // ���� ����� � ���� ������, ������������� �� ChaseState
        if (playerInSight)
        {
            if (stateController.GetCurrentState() is not ChaseState)
            {
                stateController.SetState(new ChaseState()); // ������� � ��������� �������������
            }
        }
        else
        {
            if (stateController.GetCurrentState() is not PatrolState)
            {
                stateController.SetState(new PatrolState()); // ������� � ��������� ��������������
            }
        }

        stateController.UpdateController(); // ��������� ���������� ���������
    }

    // ��������, ����� �� ���� ������
    private void CheckPlayerSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // ���� ����� ������� ������, �� ����� ���
        if (distanceToPlayer > detectionRadius)
        {
            playerInSight = false;
            return;
        }

        // ���������, �������� �� ����� � ���� ���� ������
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            // ���������, �� ��������� �� ���-�� ����� ������ � ������� (������� ��������)
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRadius, targetLayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    playerInSight = true; // ����� �����
                    return;
                }
            }
        }

        playerInSight = false; // ����� �� � ���� ������
    }
}
