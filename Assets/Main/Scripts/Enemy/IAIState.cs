public interface IAIState
{
    void EnterState(StateController owner);

    void UpdateState();
    string GetStateName();
    void ExitState();
}
