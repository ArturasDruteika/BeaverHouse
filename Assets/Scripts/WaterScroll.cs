using UnityEngine;

public class WaterScroll : MonoBehaviour
{
    public Vector2 ScrollSpeed = new Vector2(0.02f, 0.01f);

    private Renderer _renderer;
    private Vector2 _offset;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _offset += ScrollSpeed * Time.deltaTime;
        _renderer.material.mainTextureOffset = _offset;
    }
}
