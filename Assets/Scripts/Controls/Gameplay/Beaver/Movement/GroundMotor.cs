using UnityEngine;

public class GroundMotor : IMovementMotor
{
    public void Tick(
        Rigidbody rigidbody,
        Transform beaverTransform,
        Transform cameraPivot,
        BeaverInputState input,
        BeaverMovementSettings settings,
        float surfaceY
    )
    {
        rigidbody.useGravity = true;
        SetDrag(rigidbody, 0f);

        bool steerWithCamera = MovementBasis.IsSteeringWithCamera(cameraPivot);
        Transform basis = MovementBasis.GetMoveBasis(beaverTransform, cameraPivot);

        if (!MovementBasis.TryGetFlattenedBasis(basis, out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 moveDir = forward * input.Move.y + right * input.Move.x;
        if (moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        moveDir.Normalize();

        Vector3 delta = moveDir * (settings.GroundMoveSpeed * Time.fixedDeltaTime);
        rigidbody.MovePosition(rigidbody.position + delta);

        if (steerWithCamera)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            Quaternion newRot = Quaternion.RotateTowards(
                rigidbody.rotation,
                targetRot,
                settings.GroundTurnSpeed * Time.fixedDeltaTime
            );
            rigidbody.MoveRotation(newRot);
        }
    }

    private void SetDrag(Rigidbody rigidbody, float value)
    {
        rigidbody.linearDamping = value;
    }
}
