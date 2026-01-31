using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerHorizontalMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 7f;
    public int initHp = 10;
    public int hp;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1f;
    private float nextAttackTime = 0f;
    private bool attacking = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float originalScaleX;

    [SerializeField] private Animator _animator;
    [SerializeField] private HealthBar healthBar;
    public Faction faction = Faction.PlayerCharacter;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
        hp = initHp;
    }

    void Update()
    {
        // -------- Horizontal movement --------
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

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // -------- Jump --------
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        // -------- Attack with cooldown --------
        if (Keyboard.current.zKey.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            attacking = true;
            nextAttackTime = Time.time + attackCooldown;
            _animator.SetTrigger("Attack");
        }
        else
        {
            attacking = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
    }

    // Called by Animation Event at end of attack animation
    public void EndAttack()
    {
        attacking = false;
    }

    public bool IsAttacking => attacking;

    public void GetDamage(int damageValue)
    {
        hp -= damageValue; //damage
        healthBar.UpdateHealthBar(hp, initHp);
        Debug.Log($"left HP: {hp}");
        if (hp <= 0)
        {
            hp = 0;
            //Gameover logic
            return;
        }

        StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(.2f);
        renderer.color = new Color(1, 1, 1, 1);
    }
}
