using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerStalking : MonoBehaviour
{
    public float _timerBefAttack = 5;
    internal float timerBefAttack;

    public float _timeOfStalking = 20;
    internal float timeOfStalking;

    public int _stalkingLevel = 5;
    internal int stalkingLevel;

    public GameObject huntingPlayer;

    [SerializeField] public float keepingDistance;
    public float keepingDistanceMultiplier = 1;
    void Start()
    {
        timerBefAttack = _timerBefAttack;
        stalkingLevel = _stalkingLevel;
        timeOfStalking = _timeOfStalking;
    }


    private void Update()
    {
        if (timeOfStalking < 0)
        {
            stalkingLevel--;
        }
        else
        {
            timeOfStalking -= Time.deltaTime;
        }
    }

    public void ResetStalkingParam()
    {
        timerBefAttack = _timerBefAttack;
        stalkingLevel = _stalkingLevel;
        timeOfStalking = _timeOfStalking;
    }

    public static Vector3 FindBestWaypoint(Vector3 enemyPosition, Vector3 playerPosition, Vector3[] waypoints, float safeDistance)
    {
        Vector3 bestWaypoint = Vector3.zero;
        float minDistanceToEnemy = float.MaxValue;

        foreach (Vector3 waypoint in waypoints)
        {
            float distanceToEnemy = Vector3.Distance(enemyPosition, waypoint);
            float distanceToPlayer = Vector3.Distance(playerPosition, waypoint);

            if (distanceToEnemy < minDistanceToEnemy && distanceToPlayer > safeDistance)
            {
                minDistanceToEnemy = distanceToEnemy;
                bestWaypoint = waypoint;
            }
        }

        return bestWaypoint;
    }
}
