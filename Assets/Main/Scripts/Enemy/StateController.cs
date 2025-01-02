using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private IAIState currentState; // ������� ���������

    public void SetState(IAIState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(); // ��������� ������� ���������
        }

        currentState = newState; // ��������� ����� ���������

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
