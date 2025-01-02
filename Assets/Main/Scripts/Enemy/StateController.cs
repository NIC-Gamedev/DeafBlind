using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] private IAIState currentState; // Текущее состояние
    public string statea;
    public void SetState(IAIState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(); // Завершаем текущее состояние
        }

        currentState = newState; // Назначаем новое состояние
        statea = newState.GetStateName();
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
