using UnityEngine;

public class ChaseState : MonoBehaviour, IAIState
{
    private GameObject target; // ���� ��� �������������
    private EnemyMovement enemyMovement; // ������ �� ��������� EnemyMovement
    private float stopDistance = 2f; // ����������, �� ������� ���� ����������� ����� �����
    private EnemyPerception enemyPerception;
    // ����� ����� � ���������, ��������� ������ ����
    
    public void EnterState(GameObject owner)
    {
        enemyPerception = gameObject.GetComponent<EnemyPerception>();
        GameObject chaseTarget = enemyPerception.lastSeenTarget;
        enemyMovement = owner.GetComponent<EnemyMovement>();

        if (chaseTarget != null)
        {
            target = chaseTarget;
        }
        else
        {
            Debug.LogError("ChaseState: ���� ��� ������������� �� ��������!");
        }
    }

    public string GetStateName()
    {
        return "Chase";
    }

    public void UpdateState()
    {
        if (target == null || enemyMovement == null) return;

        float distanceToTarget = Vector3.Distance(enemyMovement.transform.position, target.transform.position);

        // ���� ���� � �������� ������������, ��������� � ���
        if (distanceToTarget > stopDistance)
        {
            enemyMovement.SetTarget(target);
        }
        else
        {
            // ����� ����� �������� ������, ��������, ������� � ��������� �����
            Debug.Log("���� ����������!");
        }
    }

    public void ExitState()
    {
        // ������� ���� ��� ������ �� ���������
        target = null;
    }
}
