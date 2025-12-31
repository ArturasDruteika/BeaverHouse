using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeaverController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float TurnSpeed = 360f;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // This will be called by PlayerInput (Invoke Unity Events)
    public void OnMove(InputValue movementValue)
    {
        _moveInput = movementValue.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(_moveInput.x, 0f, _moveInput.y);

        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 movement = inputDirection * MoveSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + movement);

        if (inputDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRotation,
                TurnSpeed * Time.fixedDeltaTime
            );

            _rigidbody.MoveRotation(newRotation);
        }
    }
}
