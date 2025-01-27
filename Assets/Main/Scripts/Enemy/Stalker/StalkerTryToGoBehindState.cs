using UnityEngine; 
using UnityEngine.AI;

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
    private StateController _controller;

    [SerializeField] private float playerLostDistance = 20f;

    [SerializeField] private GameObject hidingSpotObj;
    public void EnterState(StateController owner)
    {
        _controller = owner;
    }

    public void UpdateState()
    {
        var distance = Vector3.Distance(transform.position, stalking.huntingPlayer.transform.position);
        if (!stalking.IsBehindPlayer(stalking.huntingPlayer.transform))
        {
            if (distance < playerLostDistance)
            {
                enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);

                if(distance < stalking.keepingDistanceMIN)
                {
                    _controller.SetState<StalkerHuntState>();
                }
            }
            else
            {
                _controller.SetState<StalkerLostPlayer>();
            }
        }
        else
        {
            enemyMovement.KeepDistance(stalking.huntingPlayer, stalking.keepingDistance);
            if (distance < stalking.keepingDistance) 
                _controller.SetState<StalkerHuntState>();
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
