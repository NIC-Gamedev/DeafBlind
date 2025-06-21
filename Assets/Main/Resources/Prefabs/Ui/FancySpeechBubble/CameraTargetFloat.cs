using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>������ ���������� �������� ������ � ������� Transform.</summary>
public class CameraTargetFloat : MonoBehaviour
{
    [Tooltip("����� �������, ���")]
    public float duration = 1f;

    Transform cam;              // ��� Transform'� ������
    Coroutine routine;

    void Awake() => cam = Camera.main.transform;

    /// <summary>��������� ������ ������ � ������� �����.</summary>
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
/// ����������� 4�������������� + 4������?������.
/// ������� ������������� ? ������ � ����������� �����.
/// ������� ����� �����?������ ? ������ � defaultTarget.
/// </summary>
public sealed class ButtonsCameraRelay
{
    readonly (Button btn, Transform target)[] nav; // 4�������?����
    readonly Button[] reset;                       // 4�������?������
    readonly Transform defaultTarget;              // ��������#5
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

        // �������� ���������
        foreach (var (btn, targ) in nav)
            btn.onClick.AddListener(() => mover.MoveTo(targ));

        // �������� ������
        foreach (var btn in reset)
            btn.onClick.AddListener(() => mover.MoveTo(defaultTarget));
    }

    public void Dispose()
    {
        foreach (var (btn, _) in nav) btn.onClick.RemoveAllListeners();
        foreach (var btn in reset) btn.onClick.RemoveAllListeners();
    }
}