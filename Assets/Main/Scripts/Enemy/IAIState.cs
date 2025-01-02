using UnityEngine;

public interface IAIState
{
    // Метод, вызываемый при активации состояния
    void EnterState(GameObject owner);

    // Метод, вызываемый каждый кадр
    void UpdateState();

    // Метод, вызываемый при выходе из состояния
    void ExitState();
}
