using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerHorizontalMovement : MonoBehaviour
{
    private float moveSpeed = 12f;
    private float jumpForce = 15f;
    private int initHp = 10;
    private int hp;
    private bool isDead = false;

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
        if (isDead) return;
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
        if (Keyboard.current.cKey.wasPressedThisFrame && Time.time >= nextAttackTime)
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
            Die(); //Gameover logic

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

    public void Die()
    {
        // --- mask logic unchanged ---
        // GameObject tempMask = maskObj;
        // tempMask.SetActive(true);
        // tempMask.transform.SetParent(null, true);      // keep world transform
        // tempMask.transform.position = transform.position; // use world position
        // tempMask.transform.localScale = new Vector3(2f, 2f, 1f);

        if (isDead) return;

        isDead = true;
        healthBar.gameObject.SetActive(false);

        // --- fade only this object + Atk subtree ---
        List<SpriteRenderer> fadeTargets = new List<SpriteRenderer>();

        // 1. this object's renderer (if any)
        if (TryGetComponent<SpriteRenderer>(out var selfRenderer))
            fadeTargets.Add(selfRenderer);

        // 2. Atk and its children
        Transform atk = transform.Find("Blade");
        if (atk != null)
        {
            fadeTargets.AddRange(atk.GetComponentsInChildren<SpriteRenderer>());
        }

        // fade
        foreach (var r in fadeTargets)
        {
            r.DOColor(
                new Color(r.color.r, r.color.g, r.color.b, 0f),
                3f
            );
        }

        // rotation
        transform.DOLocalRotate(new Vector3(0, 0, -90f), 1f);
    }
}
