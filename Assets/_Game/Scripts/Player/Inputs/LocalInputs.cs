using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInputs : NetworkBehaviour
{
    public static LocalInputs Instance { get; private set; }

    NetworkInputData _inputData;

    [SerializeField] InputActionReference _moveActionReference;

    bool _isJumpPressed;
    bool _isFirePressed;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Instance = this;
            _inputData = new NetworkInputData();
            return;
        }

        enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            _isJumpPressed = true;
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _isFirePressed = true;
        }
    }

    public NetworkInputData GetInputs()
    {
        _inputData.xAxi = _moveActionReference.action.ReadValue<Vector2>().x;

        _inputData.Buttons.Set(ButtonType.Jump, _isJumpPressed);
        _isJumpPressed = false;

        _inputData.Buttons.Set(ButtonType.Fire, _isFirePressed);
        _isFirePressed = false;

        return _inputData;
    }
}
