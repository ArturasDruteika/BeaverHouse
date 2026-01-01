using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public Vector3 Offset = new Vector3(0f, 1f, -5f);

    void LateUpdate()
    {
        if (Player == null)
        {
            return;
        }

        transform.position = Player.position + Offset;
        transform.LookAt(Player);
    }
}
