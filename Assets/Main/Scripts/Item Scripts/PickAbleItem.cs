using FishNet.Object;
using UnityEngine;
using UnityEngine.Serialization;

public class PickAbleItem : NetworkBehaviour
{
    public InventoryItemData ItemData;
    
    [ServerRpc(RequireOwnership = false)]
    public void PickUp(InventoryHolder inventory)
    {
        if (inventory != null )
        {
            RequestPickUpServerRpc((ulong)inventory.OwnerId,inventory);
        }
    }
    [ObserversRpc]
    private void RequestPickUpServerRpc(ulong clientId,InventoryHolder inventoryHolder)
    {
        if (inventoryHolder == null || inventoryHolder.OwnerId != (int)(clientId)) return;
        HandHolder handHolder = inventoryHolder.GetComponent<HandHolder>();
        if (handHolder != null)
        {
            handHolder.TriggerHandItemChanged(ItemData.ItemPrefab);
        }
        if (inventoryHolder.InventorySystem.AddToInventory(ItemData, 1))
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