using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class UsablePipe : NetworkBehaviour , IUsable
{
    public float dmg;

    public Coroutine attackProcess;
    public LayerMask damageLayer;
    public CapsuleCollider collider;
    public GameObject currPlayer;

    [ObserversRpc]
    public void Use(GameObject owner)
    {
        Debug.Log("Used");
        if (attackProcess == null)
        {
            currPlayer = owner;
            attackProcess = StartCoroutine(AttackProcess());
        }
    }

    private IEnumerator AttackProcess()
    {
        float attackDuration = 1.2f;
        float timer = 0f;
        List<ObjectHealth> damagedObjects = new List<ObjectHealth>();
    
        while (timer < attackDuration)
        {
            timer += Time.fixedDeltaTime;
            float height = collider.height * transform.localScale.y;
            float radius = collider.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);

            Vector3 center = transform.position + collider.center;
            Vector3 up = transform.up;
            
            Vector3 point1 = center + up * (height / 2f - radius);
            Vector3 point2 = center - up * (height / 2f - radius);
            Collider[] col = Physics.OverlapCapsule(point1, point2, radius*1.5f,damageLayer);
            if (col.Length != 0)
            {
                foreach (var obj in col)
                {
                    if (obj.gameObject == currPlayer.gameObject)
                        continue;
                    ObjectHealth health = obj.GetComponent<ObjectHealth>();
                    if (health != null && !damagedObjects.Contains(health))
                    {
                        damagedObjects.Add(health);
                        health.GetDamage(dmg);
                        break;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    
        Debug.Log("AttackEnd");
        attackProcess = null;
    }
}
