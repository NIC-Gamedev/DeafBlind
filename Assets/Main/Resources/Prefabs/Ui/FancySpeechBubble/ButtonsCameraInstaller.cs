using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Единственный лёгкий MonoBehaviour ― только для привязки объектов через инспектор.
/// </summary>
public class ButtonsCameraInstaller : MonoBehaviour
{
    [Header("4 кнопки навигации")]
    public Button nav1;
    public Button nav2;
    public Button nav3;
    public Button nav4;

    [Header("4 кнопки‑сброса")]
    public Button reset1;
    public Button reset2;
    public Button reset3;
    public Button reset4;

    [Header("Точки камеры")]
    public Transform navPos1;
    public Transform navPos2;
    public Transform navPos3;
    public Transform navPos4;
    public Transform defaultPos;      // позиция #5 («ничего нет»)

    [Header("Компонент перелёта камеры")]
    public CameraTargetFloat cameraMover;

    ButtonsCameraRelay relay;

    void Awake()
    {
        relay = new ButtonsCameraRelay(
            cameraMover,
            new (Button, Transform)[]
            {
                (nav1, navPos1),
                (nav2, navPos2),
                (nav3, navPos3),
                (nav4, navPos4)
            },
            new[] { reset1, reset2, reset3, reset4 },
            defaultPos
        );
    }

    void OnDestroy() => relay.Dispose();
}
