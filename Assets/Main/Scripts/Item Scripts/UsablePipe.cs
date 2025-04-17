using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UsablePipe : MonoBehaviour , IUsable
{
    private Player_TriggerZone _playerInteractiveZone;
    public float dmg;

    public Coroutine attackProcess;
    // Start is called before the first frame update
    void Start()
    {
        _playerInteractiveZone = GetComponentInParent<Player_TriggerZone>();
    }
    
    public void Use()
    {
        Debug.Log("Used");
        if (attackProcess == null)
        {
            attackProcess = StartCoroutine(AttackProcess());
            StartCoroutine(AttackEndProcess());
        }
    }

    public IEnumerator AttackEndProcess()
    {
        yield return new WaitForSeconds(1.2f);
        StopCoroutine(attackProcess);
        attackProcess = null;
    }

    private IEnumerator AttackProcess()
    {
        while (attackProcess != null)
        {
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
            yield return new FixedUpdate();
        }
    }
}
