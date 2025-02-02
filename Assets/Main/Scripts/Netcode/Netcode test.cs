using FishNet.Managing;
using UnityEngine;
using FishNet;
using FishNet.Object;
public class Netcodetest : NetworkBehaviour
{
    private NetworkManager _networkManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}
