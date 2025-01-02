using UnityEngine;

public interface IAIStateController
{
    // ������������� ������� ���������
    void SetState(IAIState newState);

    // ���������� ������� ���������
    IAIState GetCurrentState();

    // ����� ��� ���������� ������ ���������
    void UpdateController();
}
