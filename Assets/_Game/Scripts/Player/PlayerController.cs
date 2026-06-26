using Fusion;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerWeapon))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : NetworkBehaviour
{
    PlayerMovement _movement;
    PlayerWeapon _weapon;
    PlayerHealth _health;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _weapon = GetComponent<PlayerWeapon>();
        _health = GetComponent<PlayerHealth>();

        _health.OnRespawnUpdate += (isDead) =>
        {
            enabled = !isDead;

            if (!isDead)
            {
                _movement.Teleport(transform.position + Vector3.up * 3);
            }
        };
    }

    //Autoridad de Input y/o de Estado
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;

        _movement.Move(Vector3.right * inputData.xAxi);

        if (inputData.Buttons.IsSet(ButtonType.Jump))
        {
            _movement.Jump();
        }

        if (inputData.Buttons.IsSet(ButtonType.Fire))
        {
            _weapon.ShootGameObject();
            //_weapon.ShootRaycast();
        }
    }
}
