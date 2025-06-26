using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>Плавно перемещает основную камеру к целевой Transform.</summary>
public class CameraTargetFloat : MonoBehaviour
{
    [Tooltip("Время перелёта, сек")]
    public float duration = 1f;

    Transform cam;              // кэш Transform'а камеры
    Coroutine routine;

    void Awake() => cam = Camera.main.transform;

    /// <summary>Запускает перелёт камеры к целевой точке.</summary>
    public void MoveTo(Transform target)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveRoutine(target));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cam.position = Vector3.Lerp(startPos, endPos, t);
            cam.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        cam.SetPositionAndRotation(endPos, endRot);
    }
}

/// <summary>
/// Подписывает 4 навигационных + 4 сброс?кнопки.
/// Нажатие навигационной ? перелёт к привязанной точке.
/// Нажатие любой сброс?кнопки ? перелёт к defaultTarget.
/// </summary>
public sealed class ButtonsCameraRelay
{
    readonly (Button btn, Transform target)[] nav; // 4 кнопки?цели
    readonly Button[] reset;                       // 4 кнопки?сброса
    readonly Transform defaultTarget;              // позиция #5
    readonly CameraTargetFloat mover;

    public ButtonsCameraRelay(
        CameraTargetFloat mover,
        (Button, Transform)[] navButtons,
        Button[] resetButtons,
        Transform defaultPos)
    {
        this.mover = mover;
        nav = navButtons;
        reset = resetButtons;
        defaultTarget = defaultPos;

        // подписка навигации
        foreach (var (btn, targ) in nav)
            btn.onClick.AddListener(() => mover.MoveTo(targ));

        // подписка сброса
        foreach (var btn in reset)
            btn.onClick.AddListener(() => mover.MoveTo(defaultTarget));
    }

    public void Dispose()
    {
        foreach (var (btn, _) in nav) btn.onClick.RemoveAllListeners();
        foreach (var btn in reset) btn.onClick.RemoveAllListeners();
    }
}