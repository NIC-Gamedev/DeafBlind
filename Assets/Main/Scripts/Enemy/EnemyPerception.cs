using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [SerializeField] private float visionRange = 10f; // ������ ������
    [SerializeField] private float visionAngle = 90f; // ���� ������ (�������� ���� ������)
    [SerializeField] private LayerMask detectableLayers; // ���� ��� ����������� ��������

    private List<GameObject> visibleObjects = new List<GameObject>(); // ������ �������� � ���� ������

    // ����� ��� ��������� ���� �������� � ���� ������
    public List<GameObject> GetVisibleObjects()
    {
        visibleObjects.Clear(); // ������� ������ ����� ������ �����������
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, detectableLayers); // ��������� ������� � ������� ������

        foreach (Collider hit in hits)
        {
            if (hit != null && IsInVisionAngle(hit.transform) && HasLineOfSight(hit))
            {
                visibleObjects.Add(hit.gameObject); // ��������� ������ � ������ �������
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
        if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, visionRange, detectableLayers))
        {
            return hit.collider == target; // ���������, ��� �� �������� �� ����
        }
        return false;
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
        // �������� � ������� ������ �������� � ���� ������
        List<GameObject> currentlyVisibleObjects = GetVisibleObjects();

        if (currentlyVisibleObjects.Count > 0)
        {
            Debug.Log($"������� � ���� ������ ({currentlyVisibleObjects.Count}):");
            foreach (GameObject obj in currentlyVisibleObjects)
            {
                Debug.Log($"- {obj.name} (���: {obj.tag})");
            }
        }
        else
        {
            Debug.Log("������� �������� � ���� ������.");
        }
    }
}
