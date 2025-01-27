using UnityEngine;

public class StalkerAttackState : MonoBehaviour,IAIState
{
    private EnemyPerception _enemyPerception;
    private EnemyPerception enemyPerception
    {
        get
        {
            if (_enemyPerception == null)
            {
                _enemyPerception = GetComponent<EnemyPerception>();
            }
            return _enemyPerception;
        }
    }
    private EnemyMovement _enemyMovement;
    private EnemyMovement enemyMovement
    {
        get
        {
            if (_enemyMovement == null)
            {
                _enemyMovement = GetComponent<EnemyMovement>();
            }
            return _enemyMovement;
        }
    }
    private EnemyMeleeAttack _AttackState;
    private EnemyMeleeAttack attackState
    {
        get
        {
            if (_AttackState == null)
            {
                _AttackState = GetComponent<EnemyMeleeAttack>();
            }
            return _AttackState;
        }
    }

    [SerializeField] private float unAttackDistance;
    StateController controller;

    float closestDistance = Mathf.Infinity;
    public void EnterState(StateController owner)
    {
        controller = owner;
        attackState.Attack();
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Attack";
    }

    public void UpdateState()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, unAttackDistance, attackState.attackLayer);
        if (collider.Length == 0)
        {
            controller.SetState<StalkerHuntState>();
        }
        else
        {
            closestDistance = Mathf.Infinity;
            foreach (var item in collider)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    enemyMovement.RotateToObject(item.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, unAttackDistance);
    }
}
