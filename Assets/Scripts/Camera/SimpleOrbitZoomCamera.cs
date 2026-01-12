using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleOrbitZoomCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _camera;

    [Header("Orbit")]
    [SerializeField] private float _yawSpeed = 140f;
    [SerializeField] private float _pitchSpeed = 120f;
    [SerializeField] private float _minPitch = -25f;
    [SerializeField] private float _maxPitch = 65f;

    [Header("Zoom")]
    [SerializeField] private float _distance = 6f;
    [SerializeField] private float _minDistance = 2.5f;
    [SerializeField] private float _maxDistance = 12f;
    [SerializeField] private float _zoomSpeed = 2.0f;
    [SerializeField] private float _zoomSmooth = 14f;

    [Header("Smoothing")]
    [SerializeField] private float _positionSmooth = 12f;

    [Header("Optional")]
    [SerializeField] private bool _lockCursorWhileOrbiting = false;

    private float _yaw;
    private float _pitch;

    private float _targetDistance;

    private void Awake()
    {
        if (_camera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                _camera = cam.transform;
            }
        }

        Vector3 euler = transform.rotation.eulerAngles;
        _yaw = euler.y;
        _pitch = euler.x;

        _targetDistance = _distance;
    }

    private void LateUpdate()
    {
        if (_target == null || _camera == null || Mouse.current == null)
        {
            return;
        }

        HandleOrbitInput();
        HandleZoomInput();

        transform.position = Vector3.Lerp(
            transform.position,
            _target.position,
            1f - Mathf.Exp(-_positionSmooth * Time.deltaTime)
        );

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        _distance = Mathf.Lerp(_distance, _targetDistance, 1f - Mathf.Exp(-_zoomSmooth * Time.deltaTime));

        Vector3 localPos = _camera.localPosition;
        localPos.z = -_distance;
        _camera.localPosition = localPos;
    }
    private void HandleOrbitInput()
    {
        if (Mouse.current == null)
        {
            return;
        }

        bool rightHeld = Mouse.current.rightButton.isPressed;
        bool leftHeld = Mouse.current.leftButton.isPressed;

        if (!rightHeld && !leftHeld)
        {
            if (_lockCursorWhileOrbiting)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            return;
        }

        if (_lockCursorWhileOrbiting)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Vector2 delta = Mouse.current.delta.ReadValue();

        _yaw += delta.x * _yawSpeed * Time.deltaTime;
        _pitch -= delta.y * _pitchSpeed * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
    }

    private void HandleZoomInput()
    {
        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollY) > 0.01f)
        {
            _targetDistance -= scrollY * 0.01f * _zoomSpeed;
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        }
    }
}
