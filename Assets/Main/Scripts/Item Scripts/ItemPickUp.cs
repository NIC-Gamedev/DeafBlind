using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickUp : MonoBehaviour
{
    //public float PickUpRadius = 1f;
    //public InventoryItemData ItemData;

    //private SphereCollider myCollider;

    //private void Awake()
    //{
    //    myCollider = GetComponent<SphereCollider>();
    //    //myCollider.isTrigger = true;
    //    myCollider.radius = PickUpRadius;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    var inventory = other.transform.GetComponent<InventoryHolder>();
        
    //    if (!inventory) return;

    //    if (inventory.InventorySystem.AddToInventory(ItemData, 1))
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}

    private bool playerInRange = false;
    private InventoryHolder playerInventory;
    public InventoryItemData ItemData;
    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<InventoryHolder>();

        if (inventory)
        {
            playerInRange = true;
            playerInventory = inventory;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<InventoryHolder>() == playerInventory)
        {
            playerInRange = false;
            playerInventory = null;
        }
    }

    private void Update()
    {
        if (playerInRange && playerInventory != null && Input.GetKeyDown(KeyCode.E))
        {
           

            if (!playerInventory) return;

            if (playerInventory.InventorySystem.AddToInventory(ItemData, 1))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
