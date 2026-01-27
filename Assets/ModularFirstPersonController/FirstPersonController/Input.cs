using UnityEngine;
using UnityEngine.InputSystem;

public partial class FirstPersonController : MonoBehaviour
{
    [Header("Input Actions (New Input System)")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _sprintAction;
    [SerializeField] private InputActionReference _crouchAction;
    [SerializeField] private InputActionReference _zoomAction;

    private void EnableInputActions()
    {
        EnableAction(_moveAction);
        EnableAction(_lookAction);
        EnableAction(_jumpAction);
        EnableAction(_sprintAction);
        EnableAction(_crouchAction);
        EnableAction(_zoomAction);
    }

    private void DisableInputActions()
    {
        DisableAction(_moveAction);
        DisableAction(_lookAction);
        DisableAction(_jumpAction);
        DisableAction(_sprintAction);
        DisableAction(_crouchAction);
        DisableAction(_zoomAction);
    }

    private static void EnableAction(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Enable();
        }
    }

    private static void DisableAction(InputActionReference actionRef)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Disable();
        }
    }

    private static Vector2 ReadVector2(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return Vector2.zero;
        }
        return actionRef.action.ReadValue<Vector2>();
    }

    private static bool WasPressedThisFrame(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return false;
        }
        return actionRef.action.WasPressedThisFrame();
    }

    private static bool WasReleasedThisFrame(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return false;
        }
        return actionRef.action.WasReleasedThisFrame();
    }

    private static bool IsPressed(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null)
        {
            return false;
        }
        return actionRef.action.IsPressed();
    }
}
