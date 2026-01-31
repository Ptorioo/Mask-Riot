using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 6f;
    public float jumpForce = 7f;
    public float gravity = -20f;

    private float verticalVelocity;
    private bool isGrounded;

    void Update()
    {
        // -------- Horizontal movement --------
        float move = 0f;

        int heldDirection = 1;

        bool left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;


        if (left)
        {
            heldDirection = -1;
            spriteRenderer.flipX = true;
        }
        else if (right) {
            heldDirection = 1;
            spriteRenderer.flipX = false;
        }

        move = heldDirection * ((left ? 1 : 0) + (right ? 1 : 0) > 0 ? 1 : 0);

        // -------- Ground check (simple) --------
        if (transform.position.y <= 0f)
        {
            isGrounded = true;
            verticalVelocity = 0f;

            // clamp to ground
            transform.position = new Vector3(
                transform.position.x,
                0f,
                transform.position.z
            );
        }
        else
        {
            isGrounded = false;
        }

        // -------- Jump --------
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }

        // -------- Gravity --------
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = new Vector3(
            move * moveSpeed,
            verticalVelocity,
            0f
        );

        transform.Translate(velocity * Time.deltaTime);
    }
}
