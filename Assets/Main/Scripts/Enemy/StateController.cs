using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] private IAIState currentState; // Текущее состояние
    public string statea;

    // Обновленный обобщенный метод для установки состояния
    public void SetState<T>() where T : MonoBehaviour, IAIState
    {
        // Удаляем старое состояние, если оно есть
        if (currentState != null)
        {
            currentState.ExitState(); // Завершаем текущее состояние
            Destroy(currentState as MonoBehaviour); // Удаляем компонент
        }

        // Добавляем новый компонент состояния
        currentState = gameObject.AddComponent<T>() as IAIState;

        // Устанавливаем имя состояния
        statea = currentState.GetStateName();

        // Входим в новое состояние
        currentState?.EnterState(gameObject);
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
