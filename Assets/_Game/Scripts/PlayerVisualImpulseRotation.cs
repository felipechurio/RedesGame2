using UnityEngine;

public class PlayerVisualImpulseRotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Rigidbody _visualRb;

    [SerializeField] private float _moveImpulseMultiplier = 0.35f;
    [SerializeField] private float _minVelocityChange = 0.15f;

    [SerializeField] private float _landingImpulseMultiplier = 0.6f;
    [SerializeField] private float _minLandingSpeed = 4f;

    [SerializeField] private float _jumpImpulse = 2f;

    [SerializeField] private float _angularDrag = 1.5f;

    [SerializeField] private Vector3 _visualOffset;


    private Vector3 _lastVelocity;
    private bool _wasGrounded;

    private Vector3 _startLocalPosition;

    private void Awake()
    {
        _startLocalPosition = transform.localPosition;
        _visualRb.useGravity = false;
        _visualRb.linearDamping = 0f;
        _visualRb.angularDamping = _angularDrag;
        SnapToPlayer();
    }

    private void FixedUpdate()
    {
        transform.localPosition = _startLocalPosition;
        Vector3 velocity = _playerRb.linearVelocity;
        Vector3 velocityChange = velocity - _lastVelocity;

        // Si velocity cambia agarramos un random axis y rotamos el objeto a base de eso
        if (velocityChange.magnitude > _minVelocityChange)
        {
            Vector3 randomAxis = new Vector3(-velocityChange.y, velocityChange.x, velocityChange.x).normalized;

            _visualRb.AddTorque(randomAxis * velocityChange.magnitude * _moveImpulseMultiplier, ForceMode.Impulse);
        }

        _lastVelocity = velocity;
    }

    //impulso en posicion random cuando saltamos
    public void JumpImpulse()
    {
        _visualRb.AddTorque(Random.onUnitSphere * _jumpImpulse, ForceMode.Impulse);
    }

    //en base de que tan fuerte estamos cayendo, cuando caemos aplicamos una rotacion random
    public void LandingImpulse(float fallSpeed)
    {
        if (fallSpeed < _minLandingSpeed) return;

        _visualRb.AddTorque(Random.onUnitSphere * fallSpeed * _landingImpulseMultiplier, ForceMode.Impulse);
    }

    //para cuando disparamos
    public void ShootImpulse(Vector3 shootDirection, float force)
    {
        //cross basicamente nos da el Vector perpendicular entre estos 2, asi hacemos que el recoil se aplique bien con el Torque
        Vector3 recoilAxis = Vector3.Cross(shootDirection.normalized, Vector3.up);

        if (recoilAxis == Vector3.zero)
            recoilAxis = Random.onUnitSphere;

        _visualRb.AddTorque(recoilAxis.normalized * force , ForceMode.Impulse);
    }

    public void SnapToPlayer()
    {
        if (_playerRb == null || _visualRb == null)
            return;

        Vector3 targetPosition = _playerRb.position + _visualOffset;

        _visualRb.linearVelocity = Vector3.zero;
        _visualRb.angularVelocity = Vector3.zero;

        _visualRb.position = targetPosition;
        transform.position = targetPosition;
    }

}