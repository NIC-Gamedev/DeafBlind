using System.Collections.Generic;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private float visionRange = 10f; // ������ ������
    [SerializeField] private float visionAngle = 90f; // ���� ������ (�������� ���� ������)
    [SerializeField] private LayerMask detectableLayers; // ���� ��� ����������� ��������

    [Header("�������")]
    [SerializeField] private List<GameObject> visibleObjects = new List<GameObject>(); // ������ �������� � ���� ������ (������� � ����������)
    // ������ �� StateController ��� ����� ���������
    private StateController stateController;

    private float viewAngle => Camera.main.fieldOfView * 1.5f;

    public Vector3 playerLastSeenPos;

    private void Start()
    {
        // �������� ������ �� StateController
        stateController = GetComponent<StateController>();
    }

    // ����� ��� ��������� ���� �������� � ���� ������
    public List<GameObject> GetVisibleObjects()
    {
        visibleObjects.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, detectableLayers); // ��������� ������� � ������� ������

        foreach (Collider hit in hits)
        {
            if (hit != null && IsInVisionAngle(hit.transform) && HasLineOfSight(hit))
            {
                visibleObjects.Add(hit.gameObject);
            }  
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

        if (Physics.Raycast(transform.position + Vector3.up/3, directionToTarget, out RaycastHit hit, visionRange, ~(1 << gameObject.layer))) //��������� ���������� �����, ����� ��� �� ��� ����� ��� ���� ����� ����
        {
            return hit.collider == target;
        }
        return false;
    }
    public bool IsPlayerLookingAtMe(Transform player)
    {
        Vector3 directionToEnemy = (transform.position - player.position).normalized;

        float angle = Vector3.Angle(player.forward, directionToEnemy);
        if (angle > viewAngle / 2f)
            return false;

        float distanceToEnemy = Vector3.Distance(player.position, transform.position);
        if (distanceToEnemy > visionRange)
            return false;

        if (Physics.Raycast(player.position, directionToEnemy, distanceToEnemy, ~(1 << gameObject.layer)))
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
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
}
  