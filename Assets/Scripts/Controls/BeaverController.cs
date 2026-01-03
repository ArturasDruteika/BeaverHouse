using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeaverController : MonoBehaviour
{
    [Header("Mode (Debug)")]
    [SerializeField] private MovementMode _mode = MovementMode.Ground;

    [Header("Ground Movement")]
    [SerializeField] private float _groundMoveSpeed = 5f;
    [SerializeField] private float _groundTurnSpeed = 360f;

    [Header("Swim Surface Movement")]
    [SerializeField] private float _swimMoveSpeed = 7f; // faster than ground (as you asked)
    [SerializeField] private float _swimTurnSpeed = 360f;
    [SerializeField] private float _surfaceFollowSpeed = 8f;
    [SerializeField] private float _surfaceYOffset = 0.0f;
    [SerializeField] private float _swimDrag = 2.5f;

    [Header("Underwater Movement")]
    [SerializeField] private float _underwaterMoveSpeed = 6f;
    [SerializeField] private float _underwaterVerticalSpeed = 4f;
    [SerializeField] private float _underwaterDrag = 3.5f;
    [SerializeField] private float _maxDepth = 4f; // meters below surface

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

    // Called by PlayerInput (Invoke Unity Events)
    public void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    // Bind this to an action (Button). Suggested: LeftCtrl
    public void OnDive(InputValue value)
    {
        _diveHeld = value.isPressed;

        if (!_holdToDive && value.isPressed)
        {
            // Toggle mode when in water
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

    // Bind this to an action (Button). Suggested: Space
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
                // If not holding dive, we prefer surface mode when near the top.
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
        _rigidbody.linearDamping = 0f;

        Vector3 inputDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 movement = inputDirection * _groundMoveSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + movement);

        if (inputDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRotation,
                _groundTurnSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(newRotation);
        }
    }

    private void ApplySwimSurfaceMovement()
    {
        _rigidbody.useGravity = false;
        _rigidbody.linearDamping = _swimDrag;

        Vector3 inputDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 movement = inputDirection * _swimMoveSpeed * Time.fixedDeltaTime;

        Vector3 targetPosition = _rigidbody.position + movement;

        float surfaceY = GetSurfaceY() + _surfaceYOffset;
        targetPosition.y = Mathf.Lerp(_rigidbody.position.y, surfaceY, _surfaceFollowSpeed * Time.fixedDeltaTime);

        _rigidbody.MovePosition(targetPosition);

        if (inputDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRotation,
                _swimTurnSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(newRotation);
        }

        if (_holdToDive && _diveHeld)
        {
            SetMode(MovementMode.Underwater);
        }
    }

    private void ApplyUnderwaterMovement()
    {
        _rigidbody.useGravity = false;
        _rigidbody.linearDamping = _underwaterDrag;

        Vector3 horizontalDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);
        if (horizontalDirection.sqrMagnitude > 1f)
        {
            horizontalDirection.Normalize();
        }

        float vertical = 0f;
        if (_upHeld)
        {
            vertical += 1f;
        }

        if (_diveHeld)
        {
            vertical -= 1f;
        }

        Vector3 movement = horizontalDirection * _underwaterMoveSpeed;
        movement.y = vertical * _underwaterVerticalSpeed;
        movement *= Time.fixedDeltaTime;

        Vector3 targetPosition = _rigidbody.position + movement;

        float surfaceY = GetSurfaceY();
        float minY = surfaceY - _maxDepth;
        float maxY = surfaceY - 0.1f;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        _rigidbody.MovePosition(targetPosition);

        // Underwater rotation (face horizontal direction if moving)
        if (horizontalDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRotation,
                _swimTurnSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(newRotation);
        }

        // If not holding dive and close to surface, go back to surface mode
        if (_holdToDive && !_diveHeld)
        {
            if (Mathf.Abs(_rigidbody.position.y - surfaceY) < 0.35f)
            {
                SetMode(MovementMode.SwimSurface);
            }
        }
    }

    private float GetSurfaceY()
    {
        if (_waterSurfaceTransform == null)
        {
            // Safe fallback (but you should assign it in Inspector)
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
}
