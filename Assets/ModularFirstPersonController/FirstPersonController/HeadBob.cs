using UnityEngine;

public partial class FirstPersonController : MonoBehaviour
{
    [Header("Head Bob")]
    [SerializeField] private bool _enableHeadBob = true;
    [SerializeField] private Transform _joint;
    [SerializeField] private float _bobSpeed = 10f;
    [SerializeField] private Vector3 _bobAmount = new Vector3(.15f, .05f, 0f);

    private Vector3 _jointOriginalPos;
    private float _timer;

    private void InitializeHeadBob()
    {
        if (_joint != null)
        {
            _jointOriginalPos = _joint.localPosition;
        }
    }

    private void HandleHeadBob()
    {
        if (_joint == null)
        {
            return;
        }

        if (_isWalking)
        {
            if (_isSprinting)
            {
                _timer += Time.deltaTime * (_bobSpeed + _sprintSpeed);
            }
            else if (_isCrouched)
            {
                _timer += Time.deltaTime * (_bobSpeed * _speedReduction);
            }
            else
            {
                _timer += Time.deltaTime * _bobSpeed;
            }

            _joint.localPosition = new Vector3(
                _jointOriginalPos.x + Mathf.Sin(_timer) * _bobAmount.x,
                _jointOriginalPos.y + Mathf.Sin(_timer) * _bobAmount.y,
                _jointOriginalPos.z + Mathf.Sin(_timer) * _bobAmount.z
            );
        }
        else
        {
            _timer = 0;
            _joint.localPosition = new Vector3(
                Mathf.Lerp(_joint.localPosition.x, _jointOriginalPos.x, Time.deltaTime * _bobSpeed),
                Mathf.Lerp(_joint.localPosition.y, _jointOriginalPos.y, Time.deltaTime * _bobSpeed),
                Mathf.Lerp(_joint.localPosition.z, _jointOriginalPos.z, Time.deltaTime * _bobSpeed)
            );
        }
    }
}
