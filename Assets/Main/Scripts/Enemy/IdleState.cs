using UnityEngine;

public class IdleState : MonoBehaviour, IAIState
{
   
    public void EnterState(GameObject owner)
    {
      
    }

    // �����, ���������� ������ ����
    public void UpdateState()
    {
        
    }

    // ���������� ��� ���������
    public string GetStateName()
    {
        return "Idle";
    }

    // �����, ���������� ��� ������ �� ���������
    public void ExitState()
    {
    
    }
}
