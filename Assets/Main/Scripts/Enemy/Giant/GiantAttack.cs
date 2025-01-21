using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantAttack : MonoBehaviour
{
    public Transform attackPos;
    public float attackRadius;

    public uint damage;
    public float attackImpact;
    public float timeBeforeAttack;
    public float _timeToAttackAnimationEnd;
    private float timeToAttackAnimationEnd;

    public Coroutine attackProcess;

    public LayerMask attackLayer;
    public void Attack()
    {
        timeToAttackAnimationEnd = _timeToAttackAnimationEnd;
        if (attackProcess != null)
            return;
        attackProcess = StartCoroutine(AttackIE());
    }
    private IEnumerator AttackIE()
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

    private void FixedUpdate()
    {
        if (timeToAttackAnimationEnd > 0)
        {
            timeToAttackAnimationEnd -= Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(attackPos)
            Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}
