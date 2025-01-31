using System.Collections;
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

    private StalkerStalking _stalkerStalking;
    private StalkerStalking stalkerStalking
    {
        get
        {
            if (_stalkerStalking == null)
            {
                _stalkerStalking = GetComponent<StalkerStalking>();
            }
            return _stalkerStalking;
        }
    }
    StateController controller;

    public float attackDistance = 1;


    public Coroutine TryAttackProcces;
    public void EnterState(StateController owner)
    {
        controller = owner;
        TryAttackProcces = StartCoroutine(GoToAttak());
    }

    public void ExitState()
    {
        enemyMovement.movementSpeedMultiplier = 1;
        TryAttackProcces = null;
    }

    public string GetStateName()
    {
        return "Attack";
    }

    public void UpdateState()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    public IEnumerator GoToAttak()
    {
        while (attackState.attackProcess == null)
        {
            yield return new WaitForFixedUpdate();
            if (Vector3.Distance(transform.position, stalkerStalking.huntingPlayer.transform.position) < attackDistance)
            {
                attackState.Attack();
            }
            else
            {
                enemyMovement.movementSpeedMultiplier = 8f;
                enemyMovement.SetTarget(stalkerStalking.huntingPlayer.transform);
            }
        }

        controller.SetState<StalkerHideState>();
    }
}
