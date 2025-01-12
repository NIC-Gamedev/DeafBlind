using UnityEngine;

public class PsychoPatrolState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    public LayerMask detectLayer;
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
    private StateController controller;

    public void EnterState(StateController owner)
    {
        controller = owner;
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Patrol";
    }

    public void UpdateState()
    {
        var distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z) , new Vector2(enemyMovement.targetPosition.x, enemyMovement.targetPosition.z));
        if (distance < 0.2)
        {
            controller.SetState<PsychoIdleState>();
        }

        if (enemyPerception.GetVisibleObjects().Count > 0)
        {
            controller.SetState<PsychoChaseState>();
        }
    }
}
