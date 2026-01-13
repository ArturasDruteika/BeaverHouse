// MovementBasis.cs
using UnityEngine;
using UnityEngine.InputSystem;

public static class MovementBasis
{
    public static bool IsSteeringWithCamera(Transform cameraPivot)
    {
        return Mouse.current != null && Mouse.current.rightButton.isPressed && cameraPivot != null;
    }

    // Movement direction basis:
    // - If RMB held: move relative to camera pivot
    // - Otherwise: move relative to beaver transform (so LMB camera rotate does NOT affect movement)
    public static Transform GetMoveBasis(Transform beaverTransform, Transform cameraPivot)
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed && cameraPivot != null)
        {
            return cameraPivot;
        }

        return beaverTransform;
    }

    public static bool TryGetFlattenedBasis(Transform basis, out Vector3 forward, out Vector3 right)
    {
        forward = basis.forward;
        forward.y = 0f;

        right = basis.right;
        right.y = 0f;

        if (forward.sqrMagnitude < 0.0001f || right.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.forward;
            right = Vector3.right;
            return false;
        }

        forward.Normalize();
        right.Normalize();
        return true;
    }
}
