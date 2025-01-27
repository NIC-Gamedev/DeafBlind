using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FireFlyAttack : EnemyDistanceAttack
{
    [SerializeField] protected ParticleSystem fireTrow;

    [SerializeField] protected float radiationRadius;

    [SerializeField] protected float attackPerSec;

    protected StateController _stateController;
    protected StateController stateController 
    {
        get
        {
            if (_stateController == null)
                _stateController = GetComponent<StateController>();
            return _stateController;
        }
        set
        {

        }
    }
    public Particle[] particle;

    protected override IEnumerator AttackIE()
    {
        particle = new Particle[fireTrow.main.maxParticles];
        while (stateController.GetCurrentState() is FireFlyAttackState)
        {
            yield return new WaitForSeconds(attackPerSec);
            fireTrow.Play();
            for (int i = 0; i < fireTrow.GetParticles(particle); i++)
            {
                Collider[] colider = Physics.OverlapSphere(particle[i].position, particle[i].GetCurrentSize(fireTrow), attackLayer);
                for (int j = 0; j < colider.Length; j++)
                {
                    if (colider[j].TryGetComponent(out ObjectHealth health))
                    {
                        health.GetDamage(damage);
                    }
                }
            }
        }
        attackProcess = null;
        fireTrow.Stop();
    }

    private void Update()
    {
        var radiactiveSphere = Physics.OverlapSphere(transform.position, radiationRadius,attackLayer);

        if (radiactiveSphere != null)
        {
            foreach (var item in radiactiveSphere)
            {
                if (TryGetComponent(out PlayerMovement playerMovement))
                {
                }
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiationRadius);

        Gizmos.color = Color.red;
        if(particle != null)
            foreach (var item in particle)
            {
                Gizmos.DrawWireSphere(item.position, item.GetCurrentSize(fireTrow));
            }
    }
}
