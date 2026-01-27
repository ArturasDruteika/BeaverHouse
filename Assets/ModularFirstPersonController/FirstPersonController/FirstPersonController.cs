using UnityEngine;

public partial class FirstPersonController : MonoBehaviour
{
    private Rigidbody _rb;

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        InitializeCameraSettings();
        InitializeMovementSettings();
        InitializeHeadBob();
    }

    private void Start()
    {
        InitializeCursorAndCrosshair();
        SetupSprintBar();
    }

    private void Update()
    {
        HandleCamera();

        HandleSprint();
        HandleJump();
        HandleCrouch();

        CheckGround();

        if (_enableHeadBob)
        {
            HandleHeadBob();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
}
