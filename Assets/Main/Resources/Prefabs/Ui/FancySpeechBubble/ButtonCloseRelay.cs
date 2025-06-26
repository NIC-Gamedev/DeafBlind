// 1. Чистый класс?ретранслятор (без MonoBehaviour)
using UnityEngine;
using UnityEngine.UI;
public class ButtonsRelayInstaller : MonoBehaviour
{
    [Header("Первые три")]
    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    [Header("Целевая кнопка")]
    public Button buttonD;

    private ButtonsRelay relay;   // сама логика

    void Awake() => relay = new ButtonsRelay(buttonA, buttonB, buttonC, buttonD);

    void OnDestroy() => relay.Dispose();  // снимаем подписки
}
public sealed class ButtonsRelay
{
    private readonly Button[] triggers;   // первые 3 кнопки
    private readonly Button target;       // 4?я, которую «жмём»

    public ButtonsRelay(Button a, Button b, Button c, Button targetButton)
    {
        triggers = new[] { a, b, c };
        target = targetButton;

        // подписываемся на onClick всех трёх
        foreach (var btn in triggers)
            btn.onClick.AddListener(OnTriggerClicked);
    }

    // обработчик
    private void OnTriggerClicked()
    {
        if(target.gameObject.active)
        target.onClick.Invoke();
    }
    // важно снять подписку при уничтожении оболочки?MonoBehaviour
    public void Dispose()
    {
        foreach (var btn in triggers)
            btn.onClick.RemoveListener(OnTriggerClicked);
    }
}


