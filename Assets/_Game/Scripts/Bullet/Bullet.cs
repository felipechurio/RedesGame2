using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifespan = 2f;

    private float _timer = 0;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        _timer += Runner.DeltaTime;

        transform.position += transform.right * _speed * Runner.DeltaTime;

        if (_timer >= _lifespan)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            player.ApplyBulletHitForce(transform.right);
            Runner.Despawn(Object);
        }
    }
}