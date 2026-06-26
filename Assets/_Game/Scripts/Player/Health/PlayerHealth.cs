using Fusion;
using System;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] int _respawnCount;
    [SerializeField] float _respawnTime;

    TickTimer _respawnTimer;
    
    [SerializeField] int _maxHealth;
    [Networked, OnChangedRender(nameof(LifeChanged))]
    int CurrentHealth { get; set; }

    [Networked, OnChangedRender(nameof(DeathChanged))]
    NetworkBool IsDead { get; set; }

    LifeBarItem _lifeBar;

    public event Action<bool> OnRespawnUpdate;
    public event Action OnLeft;

    public override void Spawned()
    {
        _lifeBar = LifeBarsHandler.Instance.CreateLifeBar(this);

        CurrentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth > 0) return;

        if (_respawnCount <= 0)
        {
            Disconnect();
            return;
        }

        _respawnCount--;

        Respawn();
    }

    void Respawn()
    {
        _respawnTimer = TickTimer.CreateFromSeconds(Runner, _respawnTime);

        IsDead = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!_respawnTimer.Expired(Runner)) return;

        _respawnTimer = TickTimer.None;

        CurrentHealth = _maxHealth;

        IsDead = false;
    }

    void DeathChanged()
    {
        OnRespawnUpdate?.Invoke(IsDead);
    }

    void LifeChanged()
    {
        _lifeBar.UpdateLifeBar(CurrentHealth, _maxHealth);
    }

    void Disconnect()
    {
        if (!HasInputAuthority)
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        Runner.Despawn(Object);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft?.Invoke();
    }
}
