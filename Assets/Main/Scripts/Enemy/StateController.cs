using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class StateController : MonoBehaviour
{
    public IAIState currentState { get; private set; }
    public MonoBehaviour startState;
    public string stateName;

    public Dictionary<System.Type, IAIState> states = new Dictionary<System.Type, IAIState>();
    private void Start()
    {
        GetStates();
        SetState(startState.GetType());
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
    public void SetState(System.Type stateType)
    {
        if (states.ContainsKey(stateType))
        {
            if (currentState != null)
                currentState.ExitState();

            currentState = states[stateType];
            stateName = currentState.GetStateName();
            currentState.EnterState(this);
        }
        else
        {
            Debug.LogWarning("State not found in the dictionary.");
        }
    }

    public IAIState GetCurrentState()
    {
        return currentState;
    }
}
