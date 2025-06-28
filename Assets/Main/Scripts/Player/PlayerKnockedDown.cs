using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;
public class PlayerKnockedDown : NetworkBehaviour
{
    [SerializeField] private float timeToDie;
    private float _currTimeToDie;
    private ObjectHealth _objectHealth;
    public UnityEvent onKnockDown;
    public UnityEvent onPlayerDeath;

    public Coroutine KnockDownProcess;

    private void Start()
    {
        _currTimeToDie = timeToDie;
        _objectHealth.OnHealthValueChange += OnKnockDown;
    }

    private void OnKnockDown(float health)
    {
        if(health > 0)
            return;
        if (KnockDownProcess == null)
        {
            onKnockDown?.Invoke();
            KnockDownProcess = StartCoroutine(KnockDown());
        }
    }

    private IEnumerator KnockDown()
    {
        while (_currTimeToDie > 0)
        {
            _currTimeToDie -= Time.deltaTime;
            yield return null;
        }
        onPlayerDeath?.Invoke();
    }
}
