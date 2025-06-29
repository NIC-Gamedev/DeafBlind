using System;
using FishNet.Object;
using UnityEngine;

public class PlayerKnockDownNetWraper : NetworkBehaviour
{
    private PlayerKnockedDown _playerKnockedDown;
    private ObjectHealth _objectHealth;
    public override void OnStartClient()
    {
        base.OnStartClient();
        TryGetComponent(out _playerKnockedDown);
        TryGetComponent(out _objectHealth);
        
        if (!_playerKnockedDown || !IsOwner)
            return;
        _objectHealth.OnHealthValueChange += OnKnockDown;
    }
    
    [ObserversRpc]
    public void OnKnockDown(float hp)
    {
        Debug.Log("Execute On All clients");
        _playerKnockedDown.OnKnockDown(hp);
    }

    private void OnDestroy()
    {
        _objectHealth.OnHealthValueChange -= OnKnockDown;
    }
}
