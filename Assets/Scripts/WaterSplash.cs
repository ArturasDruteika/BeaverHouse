using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterSplash : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem SplashPrefab;

    [Header("Tuning")]
    public float MinSpeedToSplash = 0.2f;
    public float SplashCooldownSeconds = 0.12f;
    public float WaterSurfaceY = 0.4f;
    public float SpawnHeightOffset = 0.02f;

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

        // Compute horizontal speed from position delta (works with MovePosition).
        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - _previousPosition;

        // Horizontal only (ignore vertical bobbing)
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
        TrySpawnSplash(force: true);
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

        TrySpawnSplash(force: false);
    }

    private void TrySpawnSplash(bool force)
    {
        if (SplashPrefab == null)
        {
            return;
        }

        if (!force && _cooldownRemaining > 0f)
        {
            return;
        }

        float flatSpeed = _flatSpeedFromPosition;

        if (!force && flatSpeed < MinSpeedToSplash)
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            WaterSurfaceY + SpawnHeightOffset,
            transform.position.z
        );

        ParticleSystem splash = Instantiate(SplashPrefab, spawnPosition, Quaternion.identity);
        splash.Play();
        Destroy(splash.gameObject, 2f);

        _cooldownRemaining = SplashCooldownSeconds;
    }
}
