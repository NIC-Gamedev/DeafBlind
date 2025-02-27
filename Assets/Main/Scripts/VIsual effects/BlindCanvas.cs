using System.Collections;
using Cinemachine;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class BlindCanvas : NetworkBehaviour
{
    public Canvas canvas;

    public CinemachineBrain cam;

    public CinemachineVirtualCamera virtualCamera;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    public LayerMask blindCullingMask;

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        if (IsOwner)
        {
            cam = Camera.main.GetComponent<CinemachineBrain>();
            if (cam.isActiveAndEnabled == virtualCamera)
            {
                canvas.worldCamera = Camera.main;
                Camera.main.cullingMask = blindCullingMask;
                canvas.transform.SetParent(null);

            }
        }
        else
        {
            canvas.enabled = false;
        }   
    }
}
