using Fusion;
using System;
using UnityEngine;

public class NicknameComponent : NetworkBehaviour
{
    NicknameItem _nicknameItem;

    [Networked, OnChangedRender(nameof(NicknameChanged))]
    NetworkString<_32> CurrentNickname { get; set; }

    public event Action OnLeft;

    public override void Spawned()
    {
        _nicknameItem = NicknamesHandler.Instance.CreateNickname(this);

        if (HasInputAuthority)
        {
            NetworkString<_32> loadedNickname = "Unknown Nickname";

            if (PlayerPrefs.HasKey("Nickname"))
            {
                loadedNickname = PlayerPrefs.GetString("Nickname");
            }

            RPC_LoadNickname(loadedNickname);
        }
        else
        {
            NicknameChanged();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_LoadNickname(NetworkString<_32> newNickname)
    {
        CurrentNickname = newNickname;
    }

    //Se llama en todos los clientes para mostrar el nombre elegido
    void NicknameChanged()
    {
        _nicknameItem.UpdateNickname(CurrentNickname.Value);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft?.Invoke();
    }
}