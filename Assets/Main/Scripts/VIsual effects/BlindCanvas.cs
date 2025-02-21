using Cinemachine;
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

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (IsOwner)
        {
            if (cam == null)
            {
                cam = Camera.main.GetComponent<CinemachineBrain>();
                if (cam.isActiveAndEnabled == virtualCamera)
                {
                    canvas.worldCamera = Camera.main;
                    Camera.main.cullingMask = blindCullingMask;
                    canvas.transform.SetParent(null);

                }
            }
        }
        else
        {
            canvas.enabled = false;
        }
    }
}
