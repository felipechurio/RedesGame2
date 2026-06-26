using Fusion;
using Fusion.Addons.Physics;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (collision.gameObject.TryGetComponent(out IDamager damager))
        {
            Debug.Log("Damage platform touched");
            RespawnAtCheckpoint();
        }
    }

    private void RespawnAtCheckpoint()
    {
        if (_checkpoints == null)
            return;

        _checkpoints.SpawnCheckpoint(gameObject, _networkRgbd);
    }

    [SerializeField] private PlayerVisualImpulseRotation _visualImpulse;

    [SerializeField] InputActionReference _moveInput;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _acceleration = 80f;
    [SerializeField] float _deceleration = 100f;
    [SerializeField] float _turnSpeed = 140f;

    [SerializeField] float _fallGravityMultiplier = 3f;
    [SerializeField] float _lowJumpGravityMultiplier = 2f;
    [SerializeField] float _maxFallSpeed = 25f;

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundLayer;

    private Checkpoints _checkpoints;

    [SerializeField] int _maxHealth;

    [SerializeField] private Transform _bulletSpawnCenter;
    [Networked, OnChangedRender(nameof(LifeChanged))] int CurrentHealth { get; set; }

   // [SerializeField] NetworkPrefabRef _bulletPrefab;
   // [SerializeField] Transform _bulletSpawnTransform;
   // [SerializeField] LayerMask _shotLayers;

    Vector2 _movement;

    NetworkRigidbody3D _networkRgbd;

    bool _isJumpPressed;
    bool _isFirePressed;

    private bool _isGrounded;
    private bool _wasGrounded;
    private float _lastFallSpeed;

    public event Action<float> OnMovement;
    public event Action<float> OnHealthChanged;
    public event Action OnShot;
    public event Action OnDespawned;


    [SerializeField] private InputActionReference _shootInput;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _shootRecoilForce = 12f;
    [SerializeField] private float _shootOffsetFromCenter = 1f;
    [SerializeField] private int _maxAirShots = 2;

    [SerializeField] private float _shootDashSpeed = 18f;
    [SerializeField] private float _shootDashDuration = 0.12f;

    private bool _isShootDashing;
    private TickTimer _shootDashTimer;
    private int _airShotsLeft;

    [SerializeField] private NetworkPrefabRef _bulletPrefab;
    [SerializeField] private float _bulletSpawnOffset = 0.75f;
    [SerializeField] private float _bulletHitForce = 10f;

    //Es como el Awake / Start y se ejecuta cuando se conecta el objeto a la red
    public override void Spawned()
    {
        _checkpoints = GetComponent<Checkpoints>();
        _networkRgbd = GetComponent<NetworkRigidbody3D>();

        if (Object.HasInputAuthority)
        {
            Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        }

        if (Object.HasStateAuthority)
        {
            CurrentHealth = _maxHealth;
            _airShotsLeft = _maxAirShots;
            gameObject.name = $"Player {Object.InputAuthority}";
        }

        OnHealthChanged?.Invoke(CurrentHealth / (float)_maxHealth);
    }

    /*public override void Render()
    {
        _movement = _moveInput.action.ReadValue<Vector2>();

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            _isJumpPressed = true;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _isJumpPressed = true;
        }

        if (_shootInput.action.WasPressedThisFrame())
        {
            _isFirePressed = true;
        }

        //_isJumpPressed |= Keyboard.current.wKey.wasPressedThisFrame;
        //_isFirePressed |= Keyboard.current.spaceKey.wasPressedThisFrame;
    }*/

    //Es como el FixedUpdate y se ejecuta cuando la sala se actualiza
    //El FixedUpdateNetwork solo se ejecuta en objetos con Autoridad de Estado y/o Input
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        CheckGrounded();

        if (!GetInput(out NetworkInputData input))
        {
            _movement = Vector2.zero;
            return;
        }

        _movement = new Vector2(input.xAxis, 0f);

        bool jumpPressed = input.Buttons.IsSet((int)ButtonType.Jump);
        bool firePressed = input.Buttons.IsSet((int)ButtonType.Fire);
        bool jumpHeld = input.Buttons.IsSet((int)ButtonType.JumpHeld);

        if (!_isShootDashing)
        {
            Movement();
            BetterGravity(jumpHeld);
        }

        if (_isShootDashing && _shootDashTimer.Expired(Runner))
        {
            _isShootDashing = false;
        }

        if (jumpPressed && _isGrounded)
        {
            Jump();
        }

        if (firePressed)
        {
            TryShootRecoil(input.AimWorldPosition);
        }
    }
    /*
    void Movement()
    {
        //transform.position += Vector3.right * (_movement.x * _moveSpeed * Runner.DeltaTime);

        //Rotamos
        if (_movement.x != 0)
        {
            transform.right = Vector3.right * Mathf.Sign(_movement.x);
        }

        //Movemos
        _networkRgbd.Rigidbody.linearVelocity += Vector3.right * (_movement.x * _moveSpeed * 10 * Runner.DeltaTime);

        OnMovement?.Invoke(_movement.x);


        if (Mathf.Abs(_networkRgbd.Rigidbody.linearVelocity.x) <= _moveSpeed) return;

        var newVelocity = _networkRgbd.Rigidbody.linearVelocity;
        newVelocity.x = _moveSpeed * _movement.x;

        _networkRgbd.Rigidbody.linearVelocity = newVelocity;
    }
    */
    void Movement()
    {
        Rigidbody rb = _networkRgbd.Rigidbody;
        Vector3 velocity = rb.linearVelocity;

        float targetSpeed = _movement.x * _moveSpeed;

        float accelRate;

        if (Mathf.Abs(_movement.x) > 0.01f)
        {
            bool changingDirection = Mathf.Sign(_movement.x) != Mathf.Sign(velocity.x) && Mathf.Abs(velocity.x) > 0.1f;

            accelRate = changingDirection ? _turnSpeed : _acceleration;
        }
        else
        {
            accelRate = _deceleration;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accelRate * Runner.DeltaTime);

        rb.linearVelocity = velocity;

        OnMovement?.Invoke(_movement.x);
    }

    void BetterGravity(bool jumpHeld)
    {
        Rigidbody rb = _networkRgbd.Rigidbody;
        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < 0)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (_fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }
        else if (velocity.y > 0 && !jumpHeld)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (_lowJumpGravityMultiplier - 1f), ForceMode.Acceleration);
        }

        velocity = rb.linearVelocity;
        velocity.y = Mathf.Max(velocity.y, -_maxFallSpeed);
        rb.linearVelocity = velocity;
    }
    /*
    void Jump()
    {
        _networkRgbd.Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }*/
    void Jump()
    {
        Vector3 velocity = _networkRgbd.Rigidbody.linearVelocity;
        velocity.y = 0f;
        _networkRgbd.Rigidbody.linearVelocity = velocity;

        _networkRgbd.Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        _visualImpulse.JumpImpulse();
    }

    void CheckGrounded()
    {
        _wasGrounded = _isGrounded;

        if (_groundCheckPoint == null)
        {
            _isGrounded = false;
            return;
        }

        _isGrounded = Physics.CheckSphere(
            _groundCheckPoint.position,
            _groundCheckRadius,
            _groundLayer,
            QueryTriggerInteraction.Ignore
        );

        if (_isGrounded)
        {
            _airShotsLeft = _maxAirShots;
            SnapVisualToPlayer();
        }

        if (!_wasGrounded && _isGrounded)
        {
            float fallSpeed = Mathf.Abs(_lastFallSpeed);

            if (_visualImpulse != null)
            {
                _visualImpulse.LandingImpulse(fallSpeed);
            }
            SnapVisualToPlayer();
        }

        _lastFallSpeed = _networkRgbd.Rigidbody.linearVelocity.y;
    }

    void TryShootRecoil(Vector3 shootPosition)
    {
        if (_airShotsLeft <= 0)
            return;

        ApplyShootRecoil(shootPosition);
        ShotGameObject(shootPosition);

        if (!_isGrounded)
        {
            _airShotsLeft--;
        }

        OnShot?.Invoke();
    }

    //Dash
    void ApplyShootRecoil(Vector3 shootPosition)
    {
        Rigidbody rb = _networkRgbd.Rigidbody;

        Vector3 directionToShootPos = shootPosition - transform.position;
        directionToShootPos.z = 0f;

        if (directionToShootPos.sqrMagnitude <= 0.001f)
            return;

        directionToShootPos.Normalize();

        // el dash se hace en la pos opuesta al click
        Vector3 recoilDirection = -directionToShootPos;

        // le sacamos toda la fuerza asi se siente como un dash
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //la fuerza del objeto es el dash
        rb.linearVelocity = recoilDirection * _shootDashSpeed;

        _isShootDashing = true;
        _shootDashTimer = TickTimer.CreateFromSeconds(Runner, _shootDashDuration);

        if (_visualImpulse != null)
        {
            _visualImpulse.ShootImpulse(directionToShootPos, _shootRecoilForce);
        }
    }

    Vector3 GetMousePositionOnPlayerPlane()
    {
        Ray ray = _playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, transform.position.z));

        if (xyPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position + transform.right;
    }

    /*
      void ShotGameObject()
      {
        Runner.Spawn(_bulletPrefab, _bulletSpawnTransform.position, _bulletSpawnTransform.rotation);
        OnShot?.Invoke();
      }*/

    void ShotGameObject(Vector3 shootPosition)
    {
        Vector3 centerPosition = GetBulletCenterPosition();

        Vector3 shootDirection = shootPosition - centerPosition;
        shootDirection.z = 0f;

        if (shootDirection.sqrMagnitude <= 0.001f)
            return;

        shootDirection.Normalize();

        Vector3 spawnPosition = centerPosition + shootDirection * _bulletSpawnOffset;

        Quaternion bulletRotation = Quaternion.FromToRotation(
            Vector3.right,
            shootDirection
        );

        Runner.Spawn(_bulletPrefab, spawnPosition, bulletRotation);
    }

    Vector3 GetBulletCenterPosition()
    {
        if (_bulletSpawnCenter != null)
            return _bulletSpawnCenter.position;

        if (_networkRgbd != null && _networkRgbd.Rigidbody != null)
            return _networkRgbd.Rigidbody.worldCenterOfMass;

        return transform.position;
    }

    public void ApplyBulletHitForce(Vector3 hitDirection)
    {
        if (!Object.HasStateAuthority)
            return;

        Rigidbody rb = _networkRgbd.Rigidbody;

        hitDirection.z = 0f;

        if (hitDirection.sqrMagnitude <= 0.001f)
            return;

        hitDirection.Normalize();

        rb.AddForce(hitDirection * _bulletHitForce, ForceMode.Impulse);

        if (_visualImpulse != null)
        {
            _visualImpulse.ShootImpulse(hitDirection, _shootRecoilForce);
        }
    }

    //Metodo agregado en la variable [Networked].
    //Se ejecuta en todos los clientes cuando la variable cambia
    void LifeChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth / (float)_maxHealth);
    }

    private void Death()
    {
        Runner.Despawn(Object);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnDespawned?.Invoke();
    }

    public void SnapVisualToPlayer()
    {
        if (_visualImpulse != null)
        {
            _visualImpulse.SnapToPlayer();
        }
    }
}
