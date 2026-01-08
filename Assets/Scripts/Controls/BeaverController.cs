using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeaverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraPivot;

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
    [SerializeField] private float _maxDepth = 8f;

    [Header("Underwater Surface Rules")]
    [SerializeField] private float _surfaceExitDistance = 0.25f;
    [SerializeField] private float _allowAboveSurface = 0.05f;

    [Header("Water References")]
    [SerializeField] private Transform _waterSurfaceTransform;

    private Rigidbody _rigidbody;

    private Vector2 _moveInput;

    private bool _diveHeld; // Ctrl
    private bool _upHeld;   // Space
    private bool _isInWater;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        ResetInputState();
    }

    private void OnDisable()
    {
        ResetInputState();
    }

    private void ResetInputState()
    {
        _moveInput = Vector2.zero;
        _diveHeld = false;
        _upHeld = false;
    }

    public void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    public void OnDive(InputValue value)
    {
        _diveHeld = value.isPressed;
    }

    public void OnUp(InputValue value)
    {
        _upHeld = value.isPressed;
    }

    private void FixedUpdate()
    {
        // Sync to physical keys to avoid stuck input when Domain Reload is off.
        if (Keyboard.current != null)
        {
            _diveHeld = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
            _upHeld = Keyboard.current.spaceKey.isPressed;
        }

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
        if (!_isInWater)
        {
            if (_mode != MovementMode.Ground)
            {
                _mode = MovementMode.Ground;
            }
            return;
        }

        if (_mode == MovementMode.Ground)
        {
            _mode = MovementMode.SwimSurface;
        }

        float surfaceY = GetSurfaceY() + _surfaceYOffset;
        float distToSurface = Mathf.Abs(_rigidbody.position.y - surfaceY);

        if (_mode == MovementMode.SwimSurface && _diveHeld)
        {
            _mode = MovementMode.Underwater;
        }
        else if (_mode == MovementMode.Underwater && !_diveHeld && distToSurface <= _surfaceExitDistance)
        {
            _mode = MovementMode.SwimSurface;
        }
    }

    private void ApplyGroundMovement()
    {
        _rigidbody.useGravity = true;
        SetDrag(0f);

        if (!TryGetCameraBasis(out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 moveDir = forward * _moveInput.y + right * _moveInput.x;
        if (moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        moveDir.Normalize();

        Vector3 delta = moveDir * (_groundMoveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + delta);

        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        Quaternion newRot = Quaternion.RotateTowards(_rigidbody.rotation, targetRot, _groundTurnSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(newRot);
    }

    private void ApplySwimSurfaceMovement()
    {
        _rigidbody.useGravity = false;
        SetDrag(_swimDrag);

        if (!TryGetCameraBasis(out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 moveDir = forward * _moveInput.y + right * _moveInput.x;

        Vector3 targetPosition = _rigidbody.position;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();

            Vector3 delta = moveDir * (_swimMoveSpeed * Time.fixedDeltaTime);
            targetPosition += delta;

            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            Quaternion newRot = Quaternion.RotateTowards(_rigidbody.rotation, targetRot, _swimTurnSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(newRot);
        }

        float surfaceY = GetSurfaceY() + _surfaceYOffset;
        targetPosition.y = Mathf.Lerp(_rigidbody.position.y, surfaceY, _surfaceFollowSpeed * Time.fixedDeltaTime);

        _rigidbody.MovePosition(targetPosition);
    }

    private void ApplyUnderwaterMovement()
    {
        _rigidbody.useGravity = false;
        SetDrag(_underwaterDrag);

        if (!TryGetCameraBasis(out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 horizontal = forward * _moveInput.y + right * _moveInput.x;

        float vertical = 0f;
        if (_upHeld) vertical += 1f;
        if (_diveHeld) vertical -= 1f;

        Vector3 moveDir = horizontal;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();
        }

        Vector3 velocity = moveDir * _underwaterMoveSpeed;
        velocity.y = vertical * _underwaterVerticalSpeed;

        Vector3 targetPosition = _rigidbody.position + velocity * Time.fixedDeltaTime;

        float surfaceY = GetSurfaceY() + _surfaceYOffset;
        float minY = surfaceY - _maxDepth;
        float maxY = surfaceY + _allowAboveSurface;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        _rigidbody.MovePosition(targetPosition);

        Vector3 flat = horizontal;
        flat.y = 0f;

        if (flat.sqrMagnitude > 0.0001f)
        {
            flat.Normalize();
            Quaternion targetRot = Quaternion.LookRotation(flat, Vector3.up);
            Quaternion newRot = Quaternion.RotateTowards(_rigidbody.rotation, targetRot, _underwaterTurnSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(newRot);
        }
    }

    private bool TryGetCameraBasis(out Vector3 forward, out Vector3 right)
    {
        Transform basis = _cameraPivot != null ? _cameraPivot : transform;

        forward = basis.forward;
        forward.y = 0f;

        right = basis.right;
        right.y = 0f;

        float fMag = forward.sqrMagnitude;
        float rMag = right.sqrMagnitude;

        if (fMag < 0.0001f || rMag < 0.0001f)
        {
            forward = Vector3.forward;
            right = Vector3.right;
            return false;
        }

        forward.Normalize();
        right.Normalize();
        return true;
    }

    private float GetSurfaceY()
    {
        if (_waterSurfaceTransform == null)
        {
            return _rigidbody.position.y;
        }

        return _waterSurfaceTransform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _isInWater = false;
        }
    }

    private void SetDrag(float value)
    {
        _rigidbody.linearDamping = value;
    }
}
