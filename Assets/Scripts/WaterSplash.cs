using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterSplash : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem SplashPrefab;

    [Header("Tuning")]
    public float MinSpeedToSplash = 0.5f;
    public float SplashCooldownSeconds = 0.12f;
    public float WaterSurfaceY = 0.4f;
    public float SpawnHeightOffset = 0.02f;

    private Rigidbody _rigidbody;
    private float _cooldownRemaining;
    private bool _isInWater;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_cooldownRemaining > 0f)
        {
            _cooldownRemaining -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Water")
        {
            return;
        }

        _isInWater = true;
        TrySpawnSplash(force: true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name != "Water")
        {
            return;
        }

        _isInWater = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isInWater)
        {
            return;
        }

        if (other.gameObject.name != "Water")
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

        float flatSpeed = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z).magnitude;
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
