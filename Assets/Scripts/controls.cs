using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private bool attacking = false;
    private float originalScaleX;

    [SerializeField] private Animator _animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        float move = 0f;

        bool left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        if (left)
        {
            move = -1f;
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (right)
        {
            move = 1f;
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        // Fire once per press; animation will finish even after release.
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            attacking = true;
            _animator.SetTrigger("Attack");
        }

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // Called by an Animation Event at the end of the attack clip
    public void EndAttack()
    {
        attacking = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
    }

    public bool IsAttacking => attacking;
}
