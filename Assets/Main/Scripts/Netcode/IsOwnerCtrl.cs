using UnityEngine;
using FishNet;
using FishNet.Object;
public class IsOwnerCtrl : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Включаем объект только для владельца
        if (!base.IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}