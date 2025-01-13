using UnityEngine;
using static UnityEditor.Progress;

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
    public float reactionTime;
    private float chaseTime;
    public bool isPlayeLook;

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
        enemyMovement.movementSpeedMultiplier = 1;
        enemyMovement.SetTarget(null);
        chaseTime = _chaseTime;
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
            distance = float.MaxValue;
            foreach (var item in visibleObject)
            {
                var currentDistance = Vector3.Distance(transform.position, item.transform.position);
                isPlayeLook = enemyPerception.IsPlayerLookingAtMe(item.transform);
                if (distance > currentDistance)
                {
                    distance = currentDistance;
                    enemyMovement.SetTarget(isPlayeLook == false ? item.transform : null);
                    enemyMovement.movementSpeedMultiplier = 1.5f;
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
