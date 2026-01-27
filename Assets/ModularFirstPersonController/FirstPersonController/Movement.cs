using UnityEngine;
using UnityEngine.UI;

public partial class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool _playerCanMove = true;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _maxVelocityChange = 10f;

    private bool _isWalking;

    [Header("Sprint")]
    [SerializeField] private bool _enableSprint = true;
    [SerializeField] private bool _unlimitedSprint = false;
    [SerializeField] private float _sprintSpeed = 7f;
    [SerializeField] private float _sprintDuration = 5f;
    [SerializeField] private float _sprintCooldown = .5f;
    [SerializeField] private float _sprintFOV = 80f;
    [SerializeField] private float _sprintFOVStepTime = 10f;

    [Header("Sprint Bar")]
    [SerializeField] private bool _useSprintBar = true;
    [SerializeField] private bool _hideBarWhenFull = true;
    [SerializeField] private Image _sprintBarBG;
    [SerializeField] private Image _sprintBar;
    [SerializeField] private float _sprintBarWidthPercent = .3f;
    [SerializeField] private float _sprintBarHeightPercent = .015f;

    private CanvasGroup _sprintBarCG;
    private bool _isSprinting;
    private float _sprintRemaining;
    private float _sprintBarWidth;
    private float _sprintBarHeight;
    private bool _isSprintCooldown;
    private float _sprintCooldownReset;

    [Header("Jump")]
    [SerializeField] private bool _enableJump = true;
    [SerializeField] private float _jumpPower = 5f;

    private bool _isGrounded;

    [Header("Crouch")]
    [SerializeField] private bool _enableCrouch = true;
    [SerializeField] private bool _holdToCrouch = true;
    [SerializeField] private float _crouchHeight = .75f;
    [SerializeField] private float _speedReduction = .5f;

    private bool _isCrouched;
    private Vector3 _originalScale;

    private void InitializeMovementSettings()
    {
        _originalScale = transform.localScale;

        if (!_unlimitedSprint)
        {
            _sprintRemaining = _sprintDuration;
            _sprintCooldownReset = _sprintCooldown;
        }
    }

    private void SetupSprintBar()
    {
        _sprintBarCG = GetComponentInChildren<CanvasGroup>();

        if (_useSprintBar)
        {
            _sprintBarBG.gameObject.SetActive(true);
            _sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            _sprintBarWidth = screenWidth * _sprintBarWidthPercent;
            _sprintBarHeight = screenHeight * _sprintBarHeightPercent;

            _sprintBarBG.rectTransform.sizeDelta = new Vector3(_sprintBarWidth, _sprintBarHeight, 0f);
            _sprintBar.rectTransform.sizeDelta = new Vector3(_sprintBarWidth - 2, _sprintBarHeight - 2, 0f);

            if (_hideBarWhenFull)
            {
                _sprintBarCG.alpha = 0;
            }
        }
        else
        {
            _sprintBarBG.gameObject.SetActive(false);
            _sprintBar.gameObject.SetActive(false);
        }
    }

    private void HandleMovement()
    {
        if (!_playerCanMove || _rb == null)
        {
            return;
        }

        Vector2 move = ReadVector2(_moveAction);
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);

        if ((targetVelocity.x != 0 || targetVelocity.z != 0) && _isGrounded)
        {
            _isWalking = true;
        }
        else
        {
            _isWalking = false;
        }

        bool sprintHeld = IsPressed(_sprintAction);

        if (_enableSprint && sprintHeld && _sprintRemaining > 0f && !_isSprintCooldown)
        {
            targetVelocity = transform.TransformDirection(targetVelocity) * _sprintSpeed;

            Vector3 velocity = _rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.y = 0;

            if (velocityChange.x != 0 || velocityChange.z != 0)
            {
                _isSprinting = true;

                if (_isCrouched)
                {
                    Crouch();
                }

                if (_hideBarWhenFull && !_unlimitedSprint)
                {
                    _sprintBarCG.alpha += 5 * Time.deltaTime;
                }
            }

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            _isSprinting = false;

            if (_hideBarWhenFull && _sprintRemaining == _sprintDuration)
            {
                _sprintBarCG.alpha -= 3 * Time.deltaTime;
            }

            targetVelocity = transform.TransformDirection(targetVelocity) * _walkSpeed;

            Vector3 velocity = _rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
            velocityChange.y = 0;

            _rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    private void HandleSprint()
    {
        if (!_enableSprint)
        {
            return;
        }

        if (_isSprinting)
        {
            _isZoomed = false;

            if (_playerCamera != null)
            {
                _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _sprintFOV, _sprintFOVStepTime * Time.deltaTime);
            }

            if (!_unlimitedSprint)
            {
                _sprintRemaining -= 1 * Time.deltaTime;
                if (_sprintRemaining <= 0)
                {
                    _isSprinting = false;
                    _isSprintCooldown = true;
                }
            }
        }
        else
        {
            _sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, _sprintDuration);
        }

        if (_isSprintCooldown)
        {
            _sprintCooldown -= 1 * Time.deltaTime;
            if (_sprintCooldown <= 0)
            {
                _isSprintCooldown = false;
            }
        }
        else
        {
            _sprintCooldown = _sprintCooldownReset;
        }

        if (_useSprintBar && !_unlimitedSprint)
        {
            float sprintRemainingPercent = _sprintRemaining / _sprintDuration;
            _sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
        }
    }

    private void HandleJump()
    {
        if (_enableJump && WasPressedThisFrame(_jumpAction) && _isGrounded)
        {
            Jump();
        }
    }

    private void HandleCrouch()
    {
        if (!_enableCrouch)
        {
            return;
        }

        bool crouchPressedThisFrame = WasPressedThisFrame(_crouchAction);
        bool crouchReleasedThisFrame = WasReleasedThisFrame(_crouchAction);

        if (crouchPressedThisFrame && !_holdToCrouch)
        {
            Crouch();
        }

        if (crouchPressedThisFrame && _holdToCrouch)
        {
            _isCrouched = false;
            Crouch();
        }
        else if (crouchReleasedThisFrame && _holdToCrouch)
        {
            _isCrouched = true;
            Crouch();
        }
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y - (transform.localScale.y * .5f),
            transform.position.z
        );

        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void Jump()
    {
        if (_isGrounded && _rb != null)
        {
            _rb.AddForce(0f, _jumpPower, 0f, ForceMode.Impulse);
            _isGrounded = false;
        }

        if (_isCrouched && !_holdToCrouch)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        if (_isCrouched)
        {
            transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
            _walkSpeed /= _speedReduction;

            _isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(_originalScale.x, _crouchHeight, _originalScale.z);
            _walkSpeed *= _speedReduction;

            _isCrouched = true;
        }
    }
}
