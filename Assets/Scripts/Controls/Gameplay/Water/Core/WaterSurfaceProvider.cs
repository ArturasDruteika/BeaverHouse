using UnityEngine;

public class WaterSurfaceProvider : MonoBehaviour
{
    [SerializeField] private Transform _waterSurfaceTransform;

    public float GetSurfaceY(Vector3 fallbackPosition)
    {
        if (_waterSurfaceTransform == null)
        {
            return fallbackPosition.y;
        }

        return _waterSurfaceTransform.position.y;
    }
}
