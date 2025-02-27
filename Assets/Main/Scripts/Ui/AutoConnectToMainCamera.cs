using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class AutoConnectToMainCamera : NetworkBehaviour
{
    public Canvas canvas;
    public float planeDist;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (!canvas)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        if (IsOwner)
        {
            canvas.worldCamera = Camera.main;
            transform.parent = null; //Все это надо что бы адекватно выглядил канвас
            canvas.planeDistance = planeDist;
        }
        else
        {
            canvas.enabled = false;
        }   
    }
}
