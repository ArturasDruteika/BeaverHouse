using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterSplash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem _splashPrefab;
    [SerializeField] private Transform _waterSurfaceTransform;

    [Header("Tuning")]
    [SerializeField] private float _minSpeedToSplash = 0.2f;
    [SerializeField] private float _splashCooldownSeconds = 0.12f;
    [SerializeField] private float _spawnHeightOffset = 0.02f;

    [Header("Surface gating (recommended)")]
    [SerializeField] private float _maxDistanceFromSurfaceToSplash = 0.35f;

    private Rigidbody _rigidbody;
    private float _cooldownRemaining;
    private bool _isInWater;

    private Vector3 _previousPosition;
    private float _flatSpeedFromPosition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _previousPosition = transform.position;
    }

    private void Update()
    {
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= Time.deltaTime;
        }

        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - _previousPosition;
        delta.y = 0f;

        if (Time.deltaTime > 0.0001f)
        {
            _flatSpeedFromPosition = delta.magnitude / Time.deltaTime;
        }
        else
        {
            _flatSpeedFromPosition = 0f;
        }

        _previousPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }

        _isInWater = true;
        TrySpawnSplash(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }

        _isInWater = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Water"))
        {
            return;
        }

        TrySpawnSplash(false);
    }

    private float GetSurfaceY()
    {
        if (_waterSurfaceTransform != null)
        {
            return _waterSurfaceTransform.position.y;
        }

        return transform.position.y; // fallback to avoid NaNs
    }

    private void TrySpawnSplash(bool force)
    {
        if (!_isInWater)
        {
            return;
        }

        if (_splashPrefab == null)
        {
            return;
        }

        if (!force && _cooldownRemaining > 0f)
        {
            return;
        }

        if (!force && _flatSpeedFromPosition < _minSpeedToSplash)
        {
            return;
        }

        float surfaceY = GetSurfaceY();

        // Only spawn splashes when you are close to the surface.
        float distToSurface = Mathf.Abs(transform.position.y - surfaceY);
        if (distToSurface > _maxDistanceFromSurfaceToSplash)
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            surfaceY + _spawnHeightOffset,
            transform.position.z
        );

        ParticleSystem splash = Instantiate(_splashPrefab, spawnPosition, Quaternion.identity);
        splash.Play();
        Destroy(splash.gameObject, 2f);

        _cooldownRemaining = _splashCooldownSeconds;
    }
}
