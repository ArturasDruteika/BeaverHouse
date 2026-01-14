using UnityEngine;

public class UnderwaterMotor : IMovementMotor
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
        SetDrag(rigidbody, settings.UnderwaterDrag);

        bool steerWithCamera = MovementBasis.IsSteeringWithCamera(cameraPivot);
        Transform basis = MovementBasis.GetMoveBasis(beaverTransform, cameraPivot);

        if (!MovementBasis.TryGetFlattenedBasis(basis, out Vector3 forward, out Vector3 right))
        {
            return;
        }

        Vector3 horizontal = forward * input.Move.y + right * input.Move.x;

        float vertical = 0f;
        if (input.UpHeld) vertical += 1f;
        if (input.DiveHeld) vertical -= 1f;

        Vector3 moveDir = horizontal;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();
        }

        Vector3 velocity = moveDir * settings.UnderwaterMoveSpeed;
        velocity.y = vertical * settings.UnderwaterVerticalSpeed;

        Vector3 targetPosition = rigidbody.position + velocity * Time.fixedDeltaTime;

        float targetSurfaceY = surfaceY + settings.SurfaceYOffset;
        float minY = targetSurfaceY - settings.MaxDepth;
        float maxY = targetSurfaceY + settings.AllowAboveSurface;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        rigidbody.MovePosition(targetPosition);

        if (steerWithCamera)
        {
            Vector3 flat = horizontal;
            flat.y = 0f;

            if (flat.sqrMagnitude > 0.0001f)
            {
                flat.Normalize();
                Quaternion targetRot = Quaternion.LookRotation(flat, Vector3.up);
                Quaternion newRot = Quaternion.RotateTowards(
                    rigidbody.rotation,
                    targetRot,
                    settings.UnderwaterTurnSpeed * Time.fixedDeltaTime
                );
                rigidbody.MoveRotation(newRot);
            }
        }
    }

    private void SetDrag(Rigidbody rigidbody, float value)
    {
        rigidbody.linearDamping = value;
    }
}
