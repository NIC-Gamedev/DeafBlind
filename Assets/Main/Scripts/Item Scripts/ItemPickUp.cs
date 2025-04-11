using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickUp : NetworkBehaviour
{


    private bool playerInRange = false;
    private InventoryHolder playerInventory;
    public InventoryItemData ItemData;
    private float pickUpRadius = 10;
    private void Start()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.radius = pickUpRadius;
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        InventoryHolder inventory = other.GetComponent<InventoryHolder>();
        if (inventory != null && inventory.IsOwner)
        {
            playerInRange = true;
            playerInventory = inventory;
            RequestPickUpServerRpc((ulong)inventory.OwnerId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<InventoryHolder>() == playerInventory)
        {

            playerInRange = false;
            playerInventory = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickUpServerRpc(ulong clientId)
    {
        if (playerInventory == null || playerInventory.OwnerId != (int)(clientId)) return;
        HandHolder handHolder = playerInventory.GetComponent<HandHolder>();
        if (handHolder != null)
        {
            handHolder.TriggerHandItemChanged(ItemData.ItemPrefab);
        }
        if (playerInventory.InventorySystem.AddToInventory(ItemData, 1))
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
    }

}