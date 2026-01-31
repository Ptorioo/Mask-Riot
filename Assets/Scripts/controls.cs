using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 6f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool attacking;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // -------- Horizontal input --------
        float move = 0f;

        bool left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        if (left)
        {
            move = -1f;
            spriteRenderer.flipX = true;
        }
        else if (right)
        {
            move = 1f;
            spriteRenderer.flipX = false;
        }

        if (Keyboard.current.zKey.isPressed)
        {
            attacking = true;
        }

        // -------- Apply horizontal movement --------
        rb.linearVelocity = new Vector2(
            move * moveSpeed,
            rb.linearVelocity.y
        );

        // -------- Jump --------
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // -------- Ground detection --------
    void OnCollisionEnter2D(Collision2D collision)
    {
        // basic ground check
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    public bool IsAttacking
    {
        get { return attacking; }
    }
}
