using UnityEngine; 

public class StalkerTryToGoBehindState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    private EnemyPerception enemyPerception
    {
        get
        {
            if (_enemyPerception == null)
            {
                _enemyPerception = GetComponent<EnemyPerception>();
            }
            return _enemyPerception;
        }
    }
    private EnemyMovement _enemyMovement;
    private EnemyMovement enemyMovement
    {
        get
        {
            if (_enemyMovement == null)
            {
                _enemyMovement = GetComponent<EnemyMovement>();
            }
            return _enemyMovement;
        }
    }

    private StalkerStalking _stalking;
    private StalkerStalking stalking
    {
        get
        {
            if (_stalking == null)
            {
                _stalking = GetComponent<StalkerStalking>();
            }
            return _stalking;
        }
    }
    private StateController controller;

    [SerializeField] private float playerLostDistance = 20f;

    [SerializeField] private GameObject hidingSpotObj;
    public void EnterState(StateController owner)
    {
        controller = owner;
    }
    public bool isFind;
    public bool IsFindMe(float radius, GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius && enemyPerception.IsPlayerLookingAtMe(player.transform))
        {
            return true;
        }
        return false;
    }
    public void UpdateState()
    {
        var distance = Vector3.Distance(transform.position, stalking.huntingPlayer.transform.position);
        if (!isFind)
            isFind = IsFindMe(stalking.revealRadius, stalking.huntingPlayer);
        if (!stalking.IsBehindPlayer(stalking.huntingPlayer.transform))
        {
            if (distance < playerLostDistance)
            {
                enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);

                if (isFind)
                {
                    controller.SetState<StalkerRevealState>();
                }
                else
                {
                    enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);
                }
            }
            else
            {
                controller.SetState<StalkerLostPlayer>();
            }
        }
        else
        {
            enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);
            if (distance < stalking.keepingDistance) 
                controller.SetState<StalkerHuntState>();
        }
    }


    public string GetStateName()
    {
        return "TryToGoBehind";
    }

    public void ExitState()
    {
        Destroy(hidingSpotObj.gameObject);
    }
}
