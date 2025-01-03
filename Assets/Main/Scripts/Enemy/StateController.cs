using UnityEngine;

public class StateController : MonoBehaviour
{
    private IAIState currentState; // Текущее состояние
    public string Stato;
    private void Start()
    {
        // Устанавливаем начальное состояние
    }

    private void Update()
    {
        // Обновляем текущее состояние каждый кадр
        currentState?.UpdateState();
    }

    public void SetState<T>() where T : MonoBehaviour, IAIState
    {
        // Если текущее состояние не null, удаляем его
        if (currentState != null)
        {
            currentState.ExitState(); // Вызываем выход из состояния
            Destroy(currentState as MonoBehaviour); // Удаляем компонент состояния
        }

        // Добавляем новое состояние как компонент
        currentState = gameObject.AddComponent<T>(); // Добавляем новый компонент состояния
        Stato = currentState.GetStateName();
        currentState.EnterState(gameObject); // Активируем состояние
    }

    public IAIState GetCurrentState()
    {
        return currentState; // Возвращаем текущее состояние
    }
}
