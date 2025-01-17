using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantPatrolState : MonoBehaviour, IAIState
{
    private EnemyPerception _enemyPerception;
    public LayerMask detectLayer;
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

    private PsychoAudioListener _audioListener;
    private PsychoAudioListener audioListener
    {
        get
        {
            if (_audioListener == null)
            {
                _audioListener = GetComponent<PsychoAudioListener>();
            }
            return _audioListener;
        }
    }
    private StateController controller;

    public void EnterState(StateController owner)
    {
        controller = owner;
    }

    public void ExitState()
    {
    }

    public string GetStateName()
    {
        return "Patrol";
    }

    public void UpdateState()
    {
        if (audioListener.lastHearAudio != null)
        {
            enemyMovement.SetTarget(audioListener.lastHearAudio);
        }
        var distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemyMovement.targetPosition.x, enemyMovement.targetPosition.z));
        if (distance < 0.2)
        {
            if (audioListener.lastHearAudio == enemyMovement.target)
            {
                Destroy(audioListener.lastHearAudio.gameObject);
            }
            controller.SetState<GiantIdleState>();
        }

        if (enemyPerception.GetVisibleObjects().Count > 0)
        {
            controller.SetState<GiantChaseState>();
        }
    }
}
