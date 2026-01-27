using UnityEngine;
using UnityEngine.UI;

public partial class FirstPersonController : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _fov = 60f;
    [SerializeField] private bool _invertCamera = false;
    [SerializeField] private bool _cameraCanMove = true;
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _maxLookAngle = 50f;

    [Header("Crosshair")]
    [SerializeField] private bool _lockCursor = true;
    [SerializeField] private bool _crosshair = true;
    [SerializeField] private Sprite _crosshairImage;
    [SerializeField] private Color _crosshairColor = Color.white;

    private float _yaw;
    private float _pitch;
    private Image _crosshairObject;

    [Header("Camera Zoom")]
    [SerializeField] private bool _enableZoom = true;
    [SerializeField] private bool _holdToZoom = false;
    [SerializeField] private float _zoomFOV = 30f;
    [SerializeField] private float _zoomStepTime = 5f;

    private bool _isZoomed;

    private void InitializeCameraSettings()
    {
        _crosshairObject = GetComponentInChildren<Image>();

        if (_playerCamera != null)
        {
            _playerCamera.fieldOfView = _fov;
        }
    }

    private void InitializeCursorAndCrosshair()
    {
        if (_lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (_crosshairObject == null)
        {
            return;
        }

        if (_crosshair)
        {
            _crosshairObject.sprite = _crosshairImage;
            _crosshairObject.color = _crosshairColor;
        }
        else
        {
            _crosshairObject.gameObject.SetActive(false);
        }
    }

    private void HandleCamera()
    {
        if (_cameraCanMove)
        {
            Vector2 look = ReadVector2(_lookAction);

            _yaw = transform.localEulerAngles.y + (look.x * _mouseSensitivity);

            if (!_invertCamera)
            {
                _pitch -= (look.y * _mouseSensitivity);
            }
            else
            {
                _pitch += (look.y * _mouseSensitivity);
            }

            _pitch = Mathf.Clamp(_pitch, -_maxLookAngle, _maxLookAngle);

            transform.localEulerAngles = new Vector3(0, _yaw, 0);

            if (_playerCamera != null)
            {
                _playerCamera.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
            }
        }

        HandleZoom();
    }

    private void HandleZoom()
    {
        if (!_enableZoom || _playerCamera == null)
        {
            return;
        }

        bool zoomPressedThisFrame = WasPressedThisFrame(_zoomAction);
        bool zoomReleasedThisFrame = WasReleasedThisFrame(_zoomAction);

        if (zoomPressedThisFrame && !_holdToZoom && !_isSprinting)
        {
            _isZoomed = !_isZoomed;
        }

        if (_holdToZoom && !_isSprinting)
        {
            if (zoomPressedThisFrame)
            {
                _isZoomed = true;
            }
            else if (zoomReleasedThisFrame)
            {
                _isZoomed = false;
            }
        }

        if (_isZoomed)
        {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _zoomFOV, _zoomStepTime * Time.deltaTime);
        }
        else if (!_isZoomed && !_isSprinting)
        {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _fov, _zoomStepTime * Time.deltaTime);
        }
    }
}
