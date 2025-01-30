using UnityEngine;

public class StalkerRevealState : IAIState
{
    public void EnterState(StateController owner)
    {
        
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Reveal";
    }

    public void UpdateState()
    {
        if (enemyPerception.IsPlayerLookingAtMe(item.transform))
        {
            stalking.timerBefAttack -= Time.deltaTime;


            if (stalking.timerBefAttack < 0)
            {
                TryAttackProcces = StartCoroutine(GoToAttak());
            }
        }
        else
        {
            controller.SetState<StalkerHideState>();
        }
    }
}