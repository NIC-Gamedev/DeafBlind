using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] private IAIState currentState; // ������� ���������
    public string statea;

    // ����������� ���������� ����� ��� ��������� ���������
    public void SetState<T>() where T : MonoBehaviour, IAIState
    {
        // ������� ������ ���������, ���� ��� ����
        if (currentState != null)
        {
            currentState.ExitState(); // ��������� ������� ���������
            Destroy(currentState as MonoBehaviour); // ������� ���������
        }

        // ��������� ����� ��������� ���������
        currentState = gameObject.AddComponent<T>() as IAIState;

        // ������������� ��� ���������
        statea = currentState.GetStateName();

        // ������ � ����� ���������
        currentState?.EnterState(gameObject);
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
