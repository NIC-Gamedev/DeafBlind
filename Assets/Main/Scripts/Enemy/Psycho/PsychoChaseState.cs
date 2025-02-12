using UnityEngine;

public class PsychoChaseState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    float firstDistance = float.MaxValue;
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
            firstDistance = float.MaxValue;
            isPlayeLook = false;
            foreach (var visionRange in enemyPerception.visionRangeObjects)
            {
                if (isPlayeLook)
                    break;
                isPlayeLook = enemyPerception.IsPlayerLookingAtMe(visionRange.transform);
            }
            foreach (var item in visibleObject)
            {
                var currentDistance = Vector3.Distance(transform.position, item.transform.position);
                if (firstDistance > currentDistance)
                {
                    firstDistance = currentDistance;
                    enemyMovement.SetTarget(item.transform);
                    enemyMovement.RotateToObject(item);
                    enemyPerception.playerLastSeenPos = item.transform.position;
                    enemyMovement.movementSpeedMultiplier = isPlayeLook == false ? 1.5f : 0;
                    if (Vector3.Distance(transform.position,item.transform.position) < 1 && !isPlayeLook)
                    {
                        controller.SetState<PsychoAttackState>();
                    }
                }
            }
            if (visibleObject.Count == 0)
            {
                chaseTime -= Time.deltaTime;
                if(chaseTime < 0)
                {
                    controller.SetState<PsychoLostPlayerState>();
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
