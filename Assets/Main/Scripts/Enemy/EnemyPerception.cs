using UnityEngine;
using System.Collections.Generic;

public class EnemyPerception : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private float visionRange = 10f; // ������ ������
    [SerializeField] private float visionAngle = 90f; // ���� ������ (�������� ���� ������)
    [SerializeField] private LayerMask detectableLayers; // ���� ��� ����������� ��������

    [Header("�������")]
    [SerializeField] private List<GameObject> visibleObjects = new List<GameObject>(); // ������ �������� � ���� ������ (������� � ����������)

    private StateController stateController; // ������ �� ���������� ���������

    public GameObject lastSeenTarget; // ��������� ������� ����

    // ����� ��� ��������� ���� �������� � ���� ������
    public List<GameObject> GetVisibleObjects()
    {
        visibleObjects.Clear(); // ������� ������ ����� ������ �����������
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, detectableLayers); // ��������� ������� � ������� ������

        GameObject currentTarget = null; // ������� ����

        foreach (Collider hit in hits)
        {
            if (hit != null && IsInVisionAngle(hit.transform) && HasLineOfSight(hit))
            {
                visibleObjects.Add(hit.gameObject); // ��������� ������ � ������ �������

                // ���� ������ �����, ������������� ��� ��� ������� ����
                currentTarget = hit.gameObject;

                // ���� ���� ���, ������ ��������� �� Chase
                if (stateController != null && lastSeenTarget != currentTarget)
                {
                    lastSeenTarget = currentTarget;
                    stateController.SetState<ChaseState>(); // ����������� ��������� �� Chase
                }
            }
        }

        // ���� ������� ���� �������, ������������ � ��������������
        if (currentTarget == null && lastSeenTarget != null)
        {
            stateController.SetState<PatrolState>(); // ����������� ��������� �� Patrol
            lastSeenTarget = null; // ���������� ��������� ����
        }

        return visibleObjects;
    }

    private bool IsInVisionAngle(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        return angleToTarget <= visionAngle; // ���������, � ���� ������ �� ������
    }

    private bool HasLineOfSight(Collider target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, visionRange, detectableLayers))
        {
            return hit.collider == target; // ���������, ��� �� �������� �� ����
        }
        return false;
    }

    private void Awake()
    {
        // �������� ������ �� StateController
        stateController = GetComponent<StateController>();
    }

    private void OnDrawGizmosSelected()
    {
        // ������ ������ � ���� ������ ��� ������������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward * visionRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    private void Update()
    {
        // ��������� ������ �������� � ���� ������
        GetVisibleObjects();
    }
}
