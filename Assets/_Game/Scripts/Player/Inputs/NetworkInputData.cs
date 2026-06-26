using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float xAxis;

    public Vector3 AimWorldPosition;

    public NetworkButtons Buttons;
}

public enum ButtonType
{
    Jump = 0,
    Fire = 1,
    JumpHeld = 2
}