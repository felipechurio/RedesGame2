using Fusion;
using UnityEngine;

public class PlayParticlesOnShot : NetworkBehaviour
{
    [SerializeField] PlayerWeapon _weapon;
    [SerializeField] ParticleSystem _particles;

    [Networked, OnChangedRender(nameof(PlayParticles))]
    NetworkBool HasShot { get; set; }

    private void Awake()
    {
        //_weapon.OnShot += RPC_PlayParticles;
        _weapon.OnShot += () => HasShot = !HasShot;
    }

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //void RPC_PlayParticles()
    //{
    //    _particles.Play();
    //}

    void PlayParticles()
    {
        _particles.Play();
    }
}
