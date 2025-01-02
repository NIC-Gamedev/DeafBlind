using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private IAIState currentState; // Текущее состояние

    public void SetState(IAIState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(); // Завершаем текущее состояние
        }

        currentState = newState; // Назначаем новое состояние

        if (currentState != null)
        {
            currentState.EnterState(gameObject); // Активируем новое состояние
        }
    }

    public IAIState GetCurrentState()
    {
        return currentState; // Возвращаем текущее состояние
    }

    public void UpdateController()
    {
        currentState?.UpdateState(); // Обновляем логику текущего состояния
    }
}
