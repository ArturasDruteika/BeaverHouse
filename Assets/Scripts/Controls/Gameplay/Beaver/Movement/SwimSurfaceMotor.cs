using UnityEngine;

public class SwimSurfaceMotor : IMovementMotor
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
        rigidbody.useGravity = false;
        SetDrag(rigidbody, settings.SwimDrag);

        bool steerWithCamera = MovementBasis.IsSteeringWithCamera(cameraPivot);
        Transform basis = MovementBasis.GetMoveBasis(beaverTransform, cameraPivot);

        if (!MovementBasis.TryGetFlattenedBasis(basis, out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 moveDir = forward * input.Move.y + right * input.Move.x;

        Vector3 targetPosition = rigidbody.position;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();

            Vector3 delta = moveDir * (settings.SwimMoveSpeed * Time.fixedDeltaTime);
            targetPosition += delta;

            if (steerWithCamera)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                Quaternion newRot = Quaternion.RotateTowards(
                    rigidbody.rotation,
                    targetRot,
                    settings.SwimTurnSpeed * Time.fixedDeltaTime
                );
                rigidbody.MoveRotation(newRot);
            }
        }

        float targetSurfaceY = surfaceY + settings.SurfaceYOffset;
        targetPosition.y = Mathf.Lerp(
            rigidbody.position.y,
            targetSurfaceY,
            settings.SurfaceFollowSpeed * Time.fixedDeltaTime
        );

        rigidbody.MovePosition(targetPosition);
    }

    private void SetDrag(Rigidbody rigidbody, float value)
    {
        rigidbody.linearDamping = value;
    }
}
