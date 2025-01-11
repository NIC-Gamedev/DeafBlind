using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private IAIState currentState;
    public string stateName;

    private Dictionary<System.Type, IAIState> states = new Dictionary<System.Type, IAIState>();
    private void Start()
    {
        GetStates();
        SetState<PsychoIdleState>();
    }

    private void GetStates()
    {
        states.Clear();
        foreach (var state in GetComponents<IAIState>())
        {
            states.Add(state.GetType(), state);
        }
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void SetState<T>() where T : MonoBehaviour, IAIState
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = states[typeof(T)];
        if (currentState != null)
        {
            stateName = currentState.GetStateName();
            currentState.EnterState(this);
        }
        else
        {
            Debug.LogWarning("В обьекте нет нужного состояния, добавьте его или не вызывайте");
        }
    }

    public IAIState GetCurrentState()
    {
        return currentState;
    }
}
