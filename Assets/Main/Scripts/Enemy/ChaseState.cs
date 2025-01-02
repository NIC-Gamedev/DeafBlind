using UnityEngine;

public class ChaseState : MonoBehaviour, IAIState
{
    private GameObject target; // ���� ��� �������������
    private EnemyMovement enemyMovement; // ������ �� ��������� EnemyMovement
    private float stopDistance = 2f; // ����������, �� ������� ���� ����������� ����� �����

    // ����� ����� � ���������, ��������� ������ ����
    public void EnterState(GameObject owner)
    {
       //Its a chase so theres no need for a normal enter state , we need one with chase target
    }
    public void EnterState(GameObject owner, GameObject chaseTarget)
    {
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
