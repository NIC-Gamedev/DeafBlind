using UnityEngine;

public class PsychoChaseState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    float distance = float.MaxValue;
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

    [SerializeField] private float _reactionTime;
    [SerializeField] private float _chaseTime;
    private float reactionTime;
    private float chaseTime;


    private StateController controller;
    public void EnterState(StateController owner)
    {
        controller = owner;
        reactionTime = _reactionTime;
        chaseTime = _chaseTime;
        enemyMovement.SetTarget(null);
    }

    public void ExitState()
    {
        enemyMovement.SetTarget(null);
    }

    public string GetStateName()
    {
        return "Chase";
    }

    public void UpdateState()
    {
        if (reactionTime < 0)
        {
            var visibleObject = enemyPerception.GetVisibleObjects();
            foreach (var item in visibleObject)
            {
                var currentDistance = Vector3.Distance(transform.position, item.transform.position);
                if (distance > currentDistance)
                {
                    distance = currentDistance;
                    enemyMovement.SetTarget(item.transform);
                }
            }
            if (visibleObject.Count == 0)
            {
                chaseTime -= Time.deltaTime;
                if(chaseTime < 0)
                {
                    controller.SetState<PsychoIdleState>();
                }
            }
            else
            {
                chaseTime = _chaseTime;
            }
        }
        else
        {
            reactionTime -= Time.deltaTime;
        }
    }
}
