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

        transform.position = _player.position + _offset;
        transform.LookAt(_player);
    }
}
