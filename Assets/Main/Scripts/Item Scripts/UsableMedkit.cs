using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableMedkit  : MonoBehaviour,IUsable
{
    private ItemPickUp _data;
    private ObjectHealth _health;
    // Start is called before the first frame update
    void Start()
    {
        _health = GetComponentInParent<ObjectHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Use()
    {
        _health.AddHealth(50);
    }
}
