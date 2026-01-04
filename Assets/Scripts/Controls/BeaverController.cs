using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeaverController : MonoBehaviour
{
    [Header("Mode (Debug)")]
    [SerializeField] private MovementMode _mode = MovementMode.Ground;

    [Header("Ground Movement")]
    [SerializeField] private float _groundMoveSpeed = 5f;
    [SerializeField] private float _groundTurnSpeed = 180f;

    [Header("Swim Surface Movement")]
    [SerializeField] private float _swimMoveSpeed = 7f;
    [SerializeField] private float _swimTurnSpeed = 140f;
    [SerializeField] private float _surfaceFollowSpeed = 8f;
    [SerializeField] private float _surfaceYOffset = 0.0f;
    [SerializeField] private float _swimDrag = 2.5f;

    [Header("Underwater Movement")]
    [SerializeField] private float _underwaterMoveSpeed = 6f;
    [SerializeField] private float _underwaterTurnSpeed = 140f;
    [SerializeField] private float _underwaterVerticalSpeed = 4f;
    [SerializeField] private float _underwaterDrag = 3.5f;
    [SerializeField] private float _maxDepth = 4f;

    [Header("Water References")]
    [SerializeField] private Transform _waterSurfaceTransform;

    [Header("Input (New Input System)")]
    [SerializeField] private bool _holdToDive = true;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private bool _diveHeld;
    private bool _upHeld;
    private bool _isInWater;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    public void OnDive(InputValue value)
    {
        _diveHeld = value.isPressed;

        if (!_holdToDive && value.isPressed)
        {
            if (_mode == MovementMode.SwimSurface)
            {
                SetMode(MovementMode.Underwater);
            }
            else if (_mode == MovementMode.Underwater)
            {
                SetMode(MovementMode.SwimSurface);
            }
        }
    }

    public void OnUp(InputValue value)
    {
        _upHeld = value.isPressed;
    }

    private void FixedUpdate()
    {
        UpdateModeFromContext();

        switch (_mode)
        {
            case MovementMode.Ground:
                ApplyGroundMovement();
                break;

            case MovementMode.SwimSurface:
                ApplySwimSurfaceMovement();
                break;

            case MovementMode.Underwater:
                ApplyUnderwaterMovement();
                break;
        }
    }

    private void UpdateModeFromContext()
    {
        if (_isInWater)
        {
            if (_mode == MovementMode.Ground)
            {
                SetMode(MovementMode.SwimSurface);
            }

            if (_holdToDive && _diveHeld && _mode == MovementMode.SwimSurface)
            {
                SetMode(MovementMode.Underwater);
            }

            if (_holdToDive && !_diveHeld && _mode == MovementMode.Underwater)
            {
                float surfaceY = GetSurfaceY();
                if (Mathf.Abs(_rigidbody.position.y - surfaceY) < 0.35f)
                {
                    SetMode(MovementMode.SwimSurface);
                }
            }
        }
        else
        {
            if (_mode != MovementMode.Ground)
            {
                SetMode(MovementMode.Ground);
            }
        }
    }

    private void ApplyGroundMovement()
    {
        _rigidbody.useGravity = true;
        SetDrag(0f);

        float forward = Mathf.Clamp(_moveInput.y, -1f, 1f);
        float turn = Mathf.Clamp(_moveInput.x, -1f, 1f);

        ApplyTurn(turn, _groundTurnSpeed);
        ApplyForwardMove(forward, _groundMoveSpeed);
    }

    private void ApplySwimSurfaceMovement()
    {
        _rigidbody.useGravity = false;
        SetDrag(_swimDrag);

        float forward = Mathf.Clamp(_moveInput.y, -1f, 1f);
        float turn = Mathf.Clamp(_moveInput.x, -1f, 1f);

        ApplyTurn(turn, _swimTurnSpeed);

        Vector3 targetPosition = _rigidbody.position + transform.forward * (forward * _swimMoveSpeed * Time.fixedDeltaTime);

        float surfaceY = GetSurfaceY() + _surfaceYOffset;
        targetPosition.y = Mathf.Lerp(_rigidbody.position.y, surfaceY, _surfaceFollowSpeed * Time.fixedDeltaTime);

        _rigidbody.MovePosition(targetPosition);
    }

    private void ApplyUnderwaterMovement()
    {
        _rigidbody.useGravity = false;
        SetDrag(_underwaterDrag);

        float forward = Mathf.Clamp(_moveInput.y, -1f, 1f);
        float turn = Mathf.Clamp(_moveInput.x, -1f, 1f);

        ApplyTurn(turn, _underwaterTurnSpeed);

        float vertical = 0f;
        if (_upHeld) vertical += 1f;
        if (_diveHeld) vertical -= 1f;

        Vector3 movement = transform.forward * (forward * _underwaterMoveSpeed);
        movement.y = vertical * _underwaterVerticalSpeed;
        movement *= Time.fixedDeltaTime;

        Vector3 targetPosition = _rigidbody.position + movement;

        float surfaceY = GetSurfaceY();
        targetPosition.y = Mathf.Clamp(targetPosition.y, surfaceY - _maxDepth, surfaceY - 0.1f);

        _rigidbody.MovePosition(targetPosition);
    }

    private void ApplyTurn(float turnInput, float turnSpeedDeg)
    {
        if (Mathf.Abs(turnInput) < 0.0001f)
        {
            return;
        }

        float deltaYaw = turnInput * turnSpeedDeg * Time.fixedDeltaTime;
        Quaternion newRotation = Quaternion.Euler(0f, deltaYaw, 0f) * _rigidbody.rotation;
        _rigidbody.MoveRotation(newRotation);
    }

    private void ApplyForwardMove(float forwardInput, float moveSpeed)
    {
        if (Mathf.Abs(forwardInput) < 0.0001f)
        {
            return;
        }

        Vector3 movement = transform.forward * (forwardInput * moveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);
    }

    private float GetSurfaceY()
    {
        if (_waterSurfaceTransform == null)
        {
            return _rigidbody.position.y;
        }

        return _waterSurfaceTransform.position.y;
    }

    private void SetMode(MovementMode newMode)
    {
        if (_mode == newMode)
        {
            return;
        }

        _mode = newMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }

        _isInWater = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }

        _isInWater = false;
    }

    private void SetDrag(float value)
    {
        // Use drag if your Unity does not have linearDamping.
        // If your project uses linearDamping, replace this method body accordingly.
        _rigidbody.linearDamping = value;
    }
}
