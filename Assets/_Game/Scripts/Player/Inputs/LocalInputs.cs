using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInputs : NetworkBehaviour
{
    public static LocalInputs Instance { get; private set; }

    private NetworkInputData _inputData;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveActionReference;
    [SerializeField] private InputActionReference _jumpActionReference;
    [SerializeField] private InputActionReference _fireActionReference;

    [Header("Aim")]
    [SerializeField] private Camera _camera;

    private bool _isJumpPressed;
    private bool _isFirePressed;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Instance = this;
            _inputData = new NetworkInputData();

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            return;
        }

        enabled = false;
    }

    private void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        if (WasJumpPressed())
        {
            _isJumpPressed = true;
        }

        if (WasFirePressed())
        {
            _isFirePressed = true;
        }
    }

    public NetworkInputData GetInputs()
    {
        float moveX = 0f;

        if (_moveActionReference != null)
        {
            moveX = _moveActionReference.action.ReadValue<Vector2>().x;
        }

        _inputData.xAxis = moveX;
        _inputData.AimWorldPosition = GetMouseWorldPosition();

        _inputData.Buttons.Set((int)ButtonType.Jump, _isJumpPressed);
        _inputData.Buttons.Set((int)ButtonType.Fire, _isFirePressed);
        _inputData.Buttons.Set((int)ButtonType.JumpHeld, IsJumpHeld());

        _isJumpPressed = false;
        _isFirePressed = false;

        return _inputData;
    }

    private bool WasJumpPressed()
    {
        if (_jumpActionReference != null && _jumpActionReference.action.WasPressedThisFrame())
            return true;

        if (Keyboard.current != null &&
            (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            return true;

        return false;
    }

    private bool IsJumpHeld()
    {
        if (_jumpActionReference != null && _jumpActionReference.action.IsPressed())
            return true;

        if (Keyboard.current != null &&
            (Keyboard.current.wKey.isPressed || Keyboard.current.spaceKey.isPressed))
            return true;

        return false;
    }

    private bool WasFirePressed()
    {
        if (_fireActionReference != null && _fireActionReference.action.WasPressedThisFrame())
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_camera == null || Mouse.current == null)
        {
            return transform.position + transform.right;
        }

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane xyPlane = new Plane(
            Vector3.forward,
            new Vector3(0f, 0f, transform.position.z)
        );

        if (xyPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position + transform.right;
    }
}