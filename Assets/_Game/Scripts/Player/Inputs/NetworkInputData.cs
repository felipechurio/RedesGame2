using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float xAxi;

    //public const byte JumpButton = 1;

    public NetworkButtons Buttons;
}

public enum ButtonType
{
    Jump = 0,
    Fire = 1
}