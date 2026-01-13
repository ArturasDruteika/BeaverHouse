using System;
using UnityEngine;

[Serializable]
public class BeaverMovementSettings
{
    [Header("Ground Movement")]
    public float GroundMoveSpeed = 5f;
    public float GroundTurnSpeed = 180f;

    [Header("Swim Surface Movement")]
    public float SwimMoveSpeed = 7f;
    public float SwimTurnSpeed = 140f;
    public float SurfaceFollowSpeed = 8f;
    public float SurfaceYOffset = 0.0f;
    public float SwimDrag = 2.5f;

    [Header("Underwater Movement")]
    public float UnderwaterMoveSpeed = 6f;
    public float UnderwaterTurnSpeed = 140f;
    public float UnderwaterVerticalSpeed = 4f;
    public float UnderwaterDrag = 3.5f;
    public float MaxDepth = 8f;

    [Header("Underwater Surface Rules")]
    public float SurfaceExitDistance = 0.25f;
    public float AllowAboveSurface = 0.05f;
}
