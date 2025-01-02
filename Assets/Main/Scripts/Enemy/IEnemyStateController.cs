using UnityEngine;

public interface IAIStateController
{
    // Устанавливает текущее состояние
    void SetState(IAIState newState);

    // Возвращает текущее состояние
    IAIState GetCurrentState();

    // Метод для обновления логики состояния
    void UpdateController();
}
