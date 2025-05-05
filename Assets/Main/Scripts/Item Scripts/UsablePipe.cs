using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UsablePipe : NetworkBehaviour , IUsable
{
    private Player_TriggerZone _playerInteractiveZone;
    public float dmg;

    public Coroutine attackProcess;
    // Start is called before the first frame update
    void Start()
    {
        _playerInteractiveZone = GetComponentInParent<Player_TriggerZone>();
    }
    [ObserversRpc]
    public void Use()
    {
        Debug.Log("Used");
        if (attackProcess == null)
        {
            attackProcess = StartCoroutine(AttackProcess());
        }
    }

    private IEnumerator AttackProcess()
    {
        float attackDuration = 1.2f;
        float timer = 0f;
    
        while (timer < attackDuration)
        {
            timer += Time.fixedDeltaTime;
            Debug.Log(attackProcess);
            if (_playerInteractiveZone != null)
            {
                var objectsInZone = _playerInteractiveZone.GetObjectsInZone();
                foreach (var obj in objectsInZone)
                {
                    ObjectHealth health = obj.GetComponent<ObjectHealth>();
                    if (health != null)
                    {
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
