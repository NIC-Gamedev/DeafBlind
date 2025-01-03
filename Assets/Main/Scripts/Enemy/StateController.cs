using UnityEngine;

public class StateController : MonoBehaviour
{
    private IAIState currentState; // ������� ���������
    public string Stato;
    private void Start()
    {
        // ������������� ��������� ���������
    }

    private void Update()
    {
        // ��������� ������� ��������� ������ ����
        currentState?.UpdateState();
    }

    public void SetState<T>() where T : MonoBehaviour, IAIState
    {
        // ���� ������� ��������� �� null, ������� ���
        if (currentState != null)
        {
            currentState.ExitState(); // �������� ����� �� ���������
            Destroy(currentState as MonoBehaviour); // ������� ��������� ���������
        }

        // ��������� ����� ��������� ��� ���������
        currentState = gameObject.AddComponent<T>(); // ��������� ����� ��������� ���������
        Stato = currentState.GetStateName();
        currentState.EnterState(gameObject); // ���������� ���������
    }

    public IAIState GetCurrentState()
    {
        return currentState; // ���������� ������� ���������
    }
}
