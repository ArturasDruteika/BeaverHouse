using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, -5f);

    private void LateUpdate()
    {
        if (_player == null)
        {
            return;
        }

        // Convert offset from local space to world space
        Vector3 rotatedOffset = _player.TransformDirection(_offset);

        transform.position = _player.position + rotatedOffset;
        transform.LookAt(_player.position + Vector3.up * _offset.y);
    }
}
