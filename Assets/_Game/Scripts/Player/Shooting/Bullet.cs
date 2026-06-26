using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    /*------ VARIABLES ------*/
    //Velocidad de movimiento (Moverlo con rigidbody como en Shared)
    [SerializeField] NetworkRigidbody3D _netRb;
    [SerializeField] float _movementImpulse;
    [SerializeField] int _damage;
    [SerializeField] float _lifeTime;

    TickTimer _lifeTimer;

    public override void Spawned()
    {
        _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
        _netRb.Rigidbody.AddForce(transform.right * _movementImpulse, ForceMode.VelocityChange);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_lifeTimer.Expired(Runner)) return;

        Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(_damage);
        }

        Runner.Despawn(Object);
    }
}
