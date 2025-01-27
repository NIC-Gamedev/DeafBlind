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
    bool isFind;

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
            if (stalking.huntingPlayer != null)
            {
                if (!stalking.IsBehindPlayer(stalking.huntingPlayer.transform))
                {
                    controller.SetState<StalkerTryToGoBehindState>();
                }
            }
            var visibleObject = enemyPerception.GetVisibleObjects();
            firstDistance = float.MaxValue;
            foreach (var item in visibleObject)
            {
                if(!isFind) 
                    isFind = IsFindMe(4,item);

                var currentDistance = Vector3.Distance(transform.position, item.transform.position);

                if (firstDistance > currentDistance)
                {
                    firstDistance = currentDistance;
                    stalking.huntingPlayer = item;
                    enemyPerception.playerLastSeenPos = item.transform.position;
                }

                if (isFind)
                {
                    if (enemyPerception.IsPlayerLookingAtMe(item.transform))
                    {
                        stalking.timerBefAttack -= Time.deltaTime;


                        if (StalkerStalking.FindNearestObject(visibleObject.ToArray()))
                        {
                            TryAttackProcces = StartCoroutine(GoToAttak());
                        }
                        break;
                    }
                    else
                    {
                        controller.SetState<StalkerHideState>();
                    }
                }
                else
                {
                    if (stalking.timeOfStalking < 0)
                    {
                        stalking.stalkingLevel--;

                        if(stalking.stalkingLevel <= 0)
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

    public IEnumerator GoToAttak()
    {
        yield return null;
    }
}
