// 1. ������ �����?������������ (��� MonoBehaviour)
using UnityEngine;
using UnityEngine.UI;
public class ButtonsRelayInstaller : MonoBehaviour
{
    [Header("������ ���")]
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    [Header("������� ������")]
    public Button buttonD;

    private ButtonsRelay relay;   // ���� ������

    void Awake() => relay = new ButtonsRelay(buttonA, buttonB, buttonC, buttonD);

    void OnDestroy() => relay.Dispose();  // ������� ��������
}
public sealed class ButtonsRelay
{
    private readonly Button[] triggers;   // ������ 3 ������
    private readonly Button target;       // 4?�, ������� ����

    public ButtonsRelay(Button a, Button b, Button c, Button targetButton)
    {
        triggers = new[] { a, b, c };
        target = targetButton;

        // ������������� �� onClick ���� ���
        foreach (var btn in triggers)
            btn.onClick.AddListener(OnTriggerClicked);
    }

    // ����������
    private void OnTriggerClicked()
    {
        if(target.gameObject.active)
        target.onClick.Invoke();
    }
    // ����� ����� �������� ��� ����������� ��������?MonoBehaviour
    public void Dispose()
    {
        foreach (var btn in triggers)
            btn.onClick.RemoveListener(OnTriggerClicked);
    }
}


