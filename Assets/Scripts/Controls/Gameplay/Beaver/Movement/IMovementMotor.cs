using UnityEngine;

public interface IMovementMotor
{
    void Tick(
        Rigidbody rigidbody,
        Transform beaverTransform,
        Transform cameraPivot,
        BeaverInputState input,
        BeaverMovementSettings settings,
        float surfaceY
    );
}
