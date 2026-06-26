using Fusion;
using System;
using UnityEngine;

public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef _bulletPrefab;

    [SerializeField] Transform _spawnPoint;

    public event Action OnShot;
    public void ShootGameObject()
    {
        if (!HasStateAuthority) return;

        Runner.Spawn(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
        OnShot?.Invoke();
    }

    public void ShootRaycast()
    {
        if (!HasStateAuthority) return;

        Debug.DrawLine(start: transform.position + Vector3.up,
                        end: transform.position + Vector3.up + transform.right * 5,
                        color: Color.red, 
                        duration: .3f);

        var raycastBool = Runner.LagCompensation.Raycast(origin: transform.position + Vector3.up,
                                                            direction: transform.right,
                                                            length: 100,
                                                            player: Object.InputAuthority,
                                                            hit: out var hitInfo);

        OnShot?.Invoke();

        if (!raycastBool) return;

        if (!hitInfo.Hitbox.Root.TryGetComponent(out PlayerHealth playerHealth)) return;

        playerHealth.TakeDamage(25);
    }
}
