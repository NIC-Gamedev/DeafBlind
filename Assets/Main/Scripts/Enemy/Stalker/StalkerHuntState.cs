using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

[DisallowMultipleComponent]
public class StalkerHuntState : MonoBehaviour,IAIState
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

    private StalkerStalking _stalking;
    private StalkerStalking stalking
    {
        get
        {
            if (_stalking == null)
            {
                _stalking = GetComponent<StalkerStalking>();
            }
            return _stalking;
        }
    }

    [SerializeField] private float _reactionTime;
    [SerializeField] private float _chaseTime;
    internal float reactionTime;
    private float chaseTime;

    private StateController controller;
    public float attackDistance = 0.6f;
    public bool isFind;

    public float revealRadius = 9;

    private Coroutine TryAttackProcces;
    public void EnterState(StateController owner)
    {
        controller = owner;
        reactionTime = _reactionTime;
        chaseTime = _chaseTime;
        stalking.ResetStalkingParam();
        enemyMovement.SetTarget(null);
        TryAttackProcces = null;
    }

    public void ExitState()
    {
        enemyMovement.movementSpeedMultiplier = 1;
        enemyMovement.SetTarget(null);
        chaseTime = _chaseTime;
        isFind = false;
        stalking.timerBefAttack = stalking._timerBefAttack;
    }

    public string GetStateName()
    {
        return "Hunt";
    }

    public bool IsFindMe(float radius,GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius && enemyPerception.IsPlayerLookingAtMe(player.transform))
        {
            return true;
        }
        return false;
    }

    public void UpdateState()
    {
        if (reactionTime < 0 && TryAttackProcces == null)
        {
            var visibleObject = enemyPerception.GetVisibleObjects();
            firstDistance = float.MaxValue;
            foreach (var item in visibleObject)
            {
                if(!isFind) 
                    isFind = IsFindMe(revealRadius, item);

                var currentDistance = Vector3.Distance(transform.position, item.transform.position);

                if (firstDistance > currentDistance && !isFind)
                {
                    firstDistance = currentDistance;
                    stalking.huntingPlayer = item;
                    enemyPerception.playerLastSeenPos = item.transform.position;
                }

                if (isFind)
                {
                    controller.SetState<StalkerRevealState>();
                }
                else
                {
                    enemyMovement.RotateToObject(stalking.huntingPlayer);

                    enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);
                    if (stalking.timeOfStalking < 0)
                    {
                        stalking.stalkingLevel--;
                        stalking.timeOfStalking = stalking._timeOfStalking;
                        if (stalking.stalkingLevel <= 0)
                        {
                            TryAttackProcces = StartCoroutine(GoToAttak());
                        }
                    }
                    else
                    {
                        stalking.timeOfStalking -= Time.deltaTime;
                    }
                }
            }
            if (stalking.huntingPlayer != null)
            {
                if (!stalking.IsBehindPlayer(stalking.huntingPlayer.transform) && !isFind)
                {
                    controller.SetState<StalkerTryToGoBehindState>();
                }
            }

            if (visibleObject.Count == 0)
            {
                chaseTime -= Time.deltaTime;
                if (chaseTime < 0)
                {
                    controller.SetState<StalkerLostPlayer>();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, revealRadius);
    }

    public IEnumerator GoToAttak()
    {
        yield return null;
        Debug.Log("DIE!!");
    }
}
