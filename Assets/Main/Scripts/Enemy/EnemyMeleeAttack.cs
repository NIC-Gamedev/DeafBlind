using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : BaseAttack
{
    [SerializeField] protected Transform attackPos;
    [SerializeField] protected float attackRadius;

    [SerializeField] protected float attackImpact;
    [SerializeField] protected float timeBeforeAttack;
    [SerializeField] protected float _timeToAttackAnimationEnd;
    protected float timeToAttackAnimationEnd;


    public override void Attack()
    {
        timeToAttackAnimationEnd = _timeToAttackAnimationEnd;
        base.Attack();
    }
    protected override IEnumerator AttackIE()
    {
        yield return new WaitForSeconds(timeBeforeAttack);
        bool isGetDamage = false;
        while (timeToAttackAnimationEnd > 0)
        {
            yield return null;
            if (isGetDamage)
            {
                continue;
            }
            Collider[] colliders = Physics.OverlapSphere(attackPos.position, attackRadius, attackLayer);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out ObjectHealth health))
                {
                    health.GetDamage(damage);
                    var direction = health.transform.position - transform.position;
                    health.GetComponent<Rigidbody>().AddForce((direction - new Vector3(0, direction.y, 0)) * attackImpact, ForceMode.Impulse);
                    isGetDamage = true;
                    break;
                }
                yield return null;
            }
        }
        attackProcess = null;
    }

    protected virtual void FixedUpdate()
    {
        if (timeToAttackAnimationEnd > 0)
        {
            timeToAttackAnimationEnd -= Time.deltaTime;
        }
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
