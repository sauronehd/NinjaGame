using UnityEngine;

public class InputReader
{
    // Consider using Unity's new Input System for more robust handling
    // For now, using the legacy Input Manager

    public Vector2 GetMovementInput()
    {
        // Use GetAxisRaw for immediate response without smoothing
        float horizontal = Input.GetAxisRaw("Horizontal");
        // float vertical = Input.GetAxisRaw("Vertical"); // Ignore vertical axis for standard movement

        // Only use horizontal input for walking/running
        Vector2 input = new Vector2(horizontal, 0f);

        // Normalization might not be strictly necessary anymore with only one axis,
        // but doesn't hurt to keep if other inputs could be added later.
        // if (input.sqrMagnitude > 1) // No need to normalize a 1D vector derived this way
        // {
        //     input.Normalize();
        // }
        return input;
    }

    public bool IsRunPressed()
    {
        // Use GetKey for continuous check while held
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    public bool IsJumpPressed()
    {
        return Input.GetButtonDown("Jump");
    }

    public bool IsCrouchHeld()
    {
        // Use GetKey for continuous check while held
        // Consider making the key configurable
        return Input.GetKey(KeyCode.C);
    }

    public bool IsShootPressed()
    {
        // Use GetButtonDown for single fire per press
        // Assumes a "Fire1" button is defined (default is Left Ctrl/Mouse 0)
        return Input.GetButtonDown("Fire1");
        // If you want continuous fire while held, use GetButton("Fire1")
    }

    public bool IsKnifePressed()
    {

        // Use GetButtonDown for single fire per press
        // Assumes a "Fire2" button is defined (default is Right Ctrl/Mouse 1)
        return Input.GetKeyDown(KeyCode.Alpha8);

        // If you want continuous fire while held, use GetButton("Fire2")

    }
}