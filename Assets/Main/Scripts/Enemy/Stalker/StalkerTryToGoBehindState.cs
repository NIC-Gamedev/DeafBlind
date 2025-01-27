
using UnityEngine;
using UnityEngine.AI;

public class StalkerTryToGoBehindState : MonoBehaviour,IAIState
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

    [SerializeField] private float playerLostDistance = 20;
    public void EnterState(StateController owner)
    {
        controller = owner;
    }

    public void UpdateState()
    {
        if (!stalking.IsBehindPlayer(stalking.huntingPlayer.transform))
        {
            if(Vector3.Distance(transform.position,stalking.huntingPlayer.transform.position) < playerLostDistance)
            {
                Vector3 hidingSpot = FindHidingSpot(stalking.huntingPlayer);

                if (hidingSpot != null)
                {
                    if (hidingSpot != transform.position)
                    {
                        var hidingSpotObj = new GameObject("hidingSpot");
                        hidingSpotObj.transform.position = hidingSpot;
                        enemyMovement.SetTarget(hidingSpotObj.transform);
                    }
                    else
                    {
                        enemyMovement.MoveInDirection(transform.position - stalking.huntingPlayer.transform.position);
                    }
                }
            }
            else
            {
                controller.SetState<StalkerLostPlayer>();
            }
        }
        else
        {
            controller.SetState<StalkerHuntState>();
        }
    }

    public string GetStateName()
    {
        return "TryToGoBehind";
    }

    public void ExitState()
    {
    }

    private Vector3 FindHidingSpot(GameObject player)
    {
        Vector3 bestSpot = transform.position;
        float bestDistance = float.MaxValue;

        // ѕровер€ем несколько направлений вокруг сталкера
        for (int i = 0; i < 360; i += 45) // ѕровер€ем каждое 45 градусов
        {
            Vector3 direction = Quaternion.Euler(0, i, 0) * Vector3.forward;
            Vector3 potentialSpot = transform.position + direction * 5f; // 5 метров в выбранном направлении

            if (NavMesh.SamplePosition(potentialSpot, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                // ѕровер€ем, не видно ли эту точку игроку
                if (!IsVisibleToPlayer(player,hit.position))
                {
                    float distance = Vector3.Distance(player.transform.position, hit.position);
                    if (distance < bestDistance)
                    {
                        bestSpot = hit.position;
                        bestDistance = distance;
                    }
                }
            }
        }

        return bestSpot;
    }

    private bool IsVisibleToPlayer(GameObject player,Vector3 point)
    {
        Vector3 directionToPoint = (point - player.transform.position).normalized;
        if (Physics.Raycast(player.transform.position, directionToPoint, out RaycastHit hit, playerLostDistance))
        {
            // ≈сли точка видима игроку
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }
}
