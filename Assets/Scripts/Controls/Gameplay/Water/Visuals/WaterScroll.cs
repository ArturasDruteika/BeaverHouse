using UnityEngine;

public class WaterScroll : MonoBehaviour
{
    [SerializeField]
    private Vector2 _scrollSpeed = new Vector2(0.02f, 0.01f);

    private Renderer _renderer;
    private Vector2 _offset;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _offset += _scrollSpeed * Time.deltaTime;
        _renderer.material.mainTextureOffset = _offset;
    }
}
