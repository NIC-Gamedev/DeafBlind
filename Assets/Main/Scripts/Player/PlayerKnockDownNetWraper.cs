using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PlayerKnockDownNetWraper : NetworkBehaviour,IInteractable
{
    private PlayerKnockedDown _playerKnockedDown;
    private ObjectHealth _objectHealth;
    UnityAction _cachedAction;
    public override void OnStartClient()
    {
        base.OnStartClient();
        TryGetComponent(out _playerKnockedDown);
        TryGetComponent(out _objectHealth);
        
        if (!_playerKnockedDown || !IsOwner)
            return;
        _objectHealth.OnHealthValueChange += OnKnockDownServerRpc;
        _cachedAction = () => ApplyBaseHealthServerRpc(40);
        _playerKnockedDown.onGetUp.AddListener(_cachedAction);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void OnKnockDownServerRpc(float hp)
    {
        Debug.Log("Execute On Server");
        OnKnockDown(hp);
    }
    
    [ObserversRpc]
    public void OnKnockDown(float hp)
    {
        Debug.Log("Execute On All clients");
        _playerKnockedDown.OnKnockDown(hp);
    }
    
    [ContextMenu("Нанести 40 урона")]
    private void DebugDamageEditor()
    {
#if UNITY_EDITOR
        ApplyDamageAllClients(40f);
#endif
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyDamageAllClients(float value)
    {
        DamageEffectObserversRpc(value);
    }
    [ObserversRpc]
    private void DamageEffectObserversRpc(float value)
    {
        Debug.Log("Damage Applied");
        _objectHealth.GetDamage(value);
    }

    private void OnDestroy()
    {
        if (!IsOwner)
            return;
        _objectHealth.OnHealthValueChange -= OnKnockDown;
        _playerKnockedDown.onGetUp.RemoveListener(_cachedAction);
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnInteract()
    {
        InteractObserverRpc();
    }
    [ObserversRpc]
    public void InteractObserverRpc()
    {
        Debug.Log("Start Interact!");
        _playerKnockedDown.StartGetUp();
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnUnInteract()
    {
        Debug.Log("SERVER UN INTERACTE");
        UnInteractObserverRpc();
    }
    
    [ObserversRpc]
    public void UnInteractObserverRpc()
    {
        Debug.Log("GAG");
        _playerKnockedDown.StopGetUp();
    }
    [ServerRpc(RequireOwnership =  false)]
    public void ApplyBaseHealthServerRpc(int hp)
    {
        ApplyBaseHealthObserverRpc(hp);
    }
    [ObserversRpc]
    public void ApplyBaseHealthObserverRpc(int hp)
    {
        _objectHealth.AddHealth(hp);
    }
}
