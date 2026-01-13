using UnityEngine;
using UnityEngine.InputSystem;

public class BeaverInputState
{
    public Vector2 Move { get; private set; }
    public bool DiveHeld { get; private set; } // Ctrl
    public bool UpHeld { get; private set; }   // Space

    public void Reset()
    {
        Move = Vector2.zero;
        DiveHeld = false;
        UpHeld = false;
    }

    public void SetMove(Vector2 move)
    {
        Move = move;
    }

    public void SetDive(bool held)
    {
        DiveHeld = held;
    }

    public void SetUp(bool held)
    {
        UpHeld = held;
    }

    // Sync to physical keys to avoid stuck input when Domain Reload is off.
    public void SyncHardware()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        DiveHeld = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
        UpHeld = Keyboard.current.spaceKey.isPressed;
    }
}
