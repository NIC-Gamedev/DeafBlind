using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyChaseState : MonoBehaviour,IAIState
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
    internal float reactionTime;
    private float chaseTime;

    private StateController controller;

    public float attackDistance = 5;
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
            foreach (var item in visibleObject)
            {
                var currentDistance = Vector3.Distance(transform.position, item.transform.position);
                if (firstDistance > currentDistance)
                {
                    firstDistance = currentDistance;
                    enemyMovement.SetTarget(item.transform);
                    enemyMovement.RotateToObject(item);
                    enemyPerception.playerLastSeenPos = item.transform.position;
                    if (Vector3.Distance(transform.position, item.transform.position) < attackDistance)
                    {
                        controller.SetState<FireFlyAttackState>();
                    }
                }
            }
            if (visibleObject.Count == 0)
            {
                chaseTime -= Time.deltaTime;
                if (chaseTime < 0)
                {
                    controller.SetState<FireFlyLostPlayerState>();
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
