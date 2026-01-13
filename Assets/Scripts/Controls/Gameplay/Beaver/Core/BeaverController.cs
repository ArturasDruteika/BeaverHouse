// BeaverController.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BeaverWaterSensor))]
public class BeaverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private WaterSurfaceProvider _waterSurfaceProvider;

    [Header("Mode (Debug)")]
    [SerializeField] private MovementMode _mode = MovementMode.Ground;

    [Header("Settings")]
    [SerializeField] private BeaverMovementSettings _settings = new BeaverMovementSettings();

    private Rigidbody _rigidbody;
    private BeaverWaterSensor _waterSensor;

    private readonly BeaverInputState _input = new BeaverInputState();

    private IMovementMotor _groundMotor;
    private IMovementMotor _swimSurfaceMotor;
    private IMovementMotor _underwaterMotor;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _waterSensor = GetComponent<BeaverWaterSensor>();

        _groundMotor = new GroundMotor();
        _swimSurfaceMotor = new SwimSurfaceMotor();
        _underwaterMotor = new UnderwaterMotor();
    }

    private void OnEnable()
    {
        _input.Reset();
    }

    private void OnDisable()
    {
        _input.Reset();
    }

    public void OnMove(InputValue movementValue)
    {
        _input.SetMove(movementValue.Get<Vector2>());
    }

    public void OnDive(InputValue value)
    {
        _input.SetDive(value.isPressed);
    }

    public void OnUp(InputValue value)
    {
        _input.SetUp(value.isPressed);
    }

    private void FixedUpdate()
    {
        _input.SyncHardware();

        UpdateModeFromContext();

        float surfaceY = GetSurfaceY();

        switch (_mode)
        {
            case MovementMode.Ground:
                _groundMotor.Tick(_rigidbody, transform, _cameraPivot, _input, _settings, surfaceY);
                break;

            case MovementMode.SwimSurface:
                _swimSurfaceMotor.Tick(_rigidbody, transform, _cameraPivot, _input, _settings, surfaceY);
                break;

            case MovementMode.Underwater:
                _underwaterMotor.Tick(_rigidbody, transform, _cameraPivot, _input, _settings, surfaceY);
                break;
        }
    }

    private void UpdateModeFromContext()
    {
        if (!_waterSensor.IsInWater)
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

        float surfaceY = GetSurfaceY() + _settings.SurfaceYOffset;
        float distToSurface = Mathf.Abs(_rigidbody.position.y - surfaceY);

        if (_mode == MovementMode.SwimSurface && _input.DiveHeld)
        {
            _mode = MovementMode.Underwater;
        }
        else if (_mode == MovementMode.Underwater && !_input.DiveHeld && distToSurface <= _settings.SurfaceExitDistance)
        {
            _mode = MovementMode.SwimSurface;
        }
    }

    private float GetSurfaceY()
    {
        if (_waterSurfaceProvider == null)
        {
            return _rigidbody.position.y;
        }

        return _waterSurfaceProvider.GetSurfaceY(_rigidbody.position);
    }
}
