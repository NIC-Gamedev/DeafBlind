using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsablePipe : MonoBehaviour , IUsable
{
    private Player_TriggerZone _playerInteractiveZone;
    public float dmg;
    // Start is called before the first frame update
    void Start()
    {
        _playerInteractiveZone = GetComponentInParent<Player_TriggerZone>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Use()
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
                }
            }
        }
        else
        {
            Debug.LogWarning("PlayerInteractiveZone не назначен!");
        }
    }

    

}
