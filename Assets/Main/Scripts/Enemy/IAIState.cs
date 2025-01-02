using UnityEngine;

public interface IAIState
{
    // �����, ���������� ��� ��������� ���������
    void EnterState(GameObject owner);

    // �����, ���������� ������ ����
    void UpdateState();

    // �����, ���������� ��� ������ �� ���������
    void ExitState();
}
