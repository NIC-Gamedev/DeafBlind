using UnityEngine;

public class IdleState : MonoBehaviour, IAIState
{
   
    public void EnterState(GameObject owner)
    {
      
    }

    // Метод, вызываемый каждый кадр
    public void UpdateState()
    {
        
    }

    // Возвращаем имя состояния
    public string GetStateName()
    {
        return "Idle";
    }

    // Метод, вызываемый при выходе из состояния
    public void ExitState()
    {
    
    }
}
