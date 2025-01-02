using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] private IAIState currentState; // ������� ���������
    public string statea;
    public void SetState(IAIState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(); // ��������� ������� ���������
        }

        currentState = newState; // ��������� ����� ���������
        statea = newState.GetStateName();
        if (currentState != null)
        {
            currentState.EnterState(gameObject); // ���������� ����� ���������
        }
    }

    public IAIState GetCurrentState()
    {
        return currentState; // ���������� ������� ���������
    }

    public void UpdateController()
    {
        currentState?.UpdateState(); // ��������� ������ �������� ���������
    }
}
