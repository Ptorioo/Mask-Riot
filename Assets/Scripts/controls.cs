#region

using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

public class PlayerHorizontalMovement : MonoBehaviour

{
#region Public Variables

    // --- SINGLETON SECTION (NEW) ---

    // This allows other scripts to say "PlayerHorizontalMovement.Instance" to find me.

    public static PlayerHorizontalMovement Instance { get; private set; }

    public bool IsAttacking => attacking;

    // --- FACTION DATA (NEW) ---

    // Tracks the current mask. Defaults to PlayerCharacter (No Mask).

    public Faction faction = Faction.PlayerCharacter;

#endregion

#region Private Variables

    private float moveSpeed = 12f;

    private float jumpForce = 15f;

    private int initHp = 10;

    private int hp;

    private bool isDead = false;

    private float nextAttackTime = 0f;

    private bool attacking = false;

    private Rigidbody2D rb;

    private SpriteRenderer myRenderer; // (NEW) We cache this to change colors efficiently

    private bool isGrounded;

    private float originalScaleX;

    [Header("Attack")]
    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private SpriteRenderer maskLayer;

#endregion

#region Unity events

    void Awake()

    {
        // --- SINGLETON INITIALIZATION (NEW) ---

        if (Instance != null && Instance != this) Destroy(this);

        else Instance = this;

        rb = GetComponent<Rigidbody2D>();

        originalScaleX = Mathf.Abs(transform.localScale.x);

        hp = initHp;
    }

    void Update()

    {
        // -------- Horizontal movement --------

        if (isDead)
        {
            rb.linearVelocity = new Vector2(0 , rb.linearVelocity.y);
            return;
        }

        float move = 0f;

        bool left = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;

        bool right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;

        if (left)

        {
            move = -1f;

            transform.localScale = new Vector3(-originalScaleX , transform.localScale.y , transform.localScale.z);
        }

        else if (right)

        {
            move = 1f;

            transform.localScale = new Vector3(originalScaleX , transform.localScale.y , transform.localScale.z);
        }

        rb.linearVelocity = new Vector2(move * moveSpeed , rb.linearVelocity.y);

        // -------- Jump --------

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)

        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x , jumpForce);

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

#endregion

#region Public Methods

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
        if (TryGetComponent<SpriteRenderer>(out var selfRenderer)) fadeTargets.Add(selfRenderer);

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
                    new Color(r.color.r , r.color.g , r.color.b , 0f) ,
                    3f
                    );
        }

        // rotation
        transform.DOLocalRotate(new Vector3(0 , 0 , -90f) , 1f);
    }

    // Called by Animation Event at end of attack animation

    public void EndAttack()

    {
        attacking = false;
    }

    // --- MASK SYSTEM (UPDATED) ---
    public void EquipMask(Faction newFaction , Sprite maskSprite)
    {
        // Update the main 'faction' variable (so Enemy.cs sees the change!)
        faction = newFaction;

        if (maskLayer != null)
        {
            maskLayer.enabled = true;
            maskLayer.sprite  = maskSprite;
        }
    }

    public void GetDamage(int damageValue)

    {
        hp -= damageValue; //damage

        healthBar.UpdateHealthBar(hp , initHp);

        Debug.Log($"left HP: {hp}");

        if (hp <= 0)

        {
            hp = 0;

            // Die();

            return;
        }

        StartCoroutine(DamageEffect());
    }

#endregion

#region Private Methods

    private IEnumerator DamageEffect()

    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        renderer.color = new Color(1 , 0 , 0 , 1);

        yield return new WaitForSeconds(.2f);

        renderer.color = new Color(1 , 1 , 1 , 1);
    }

    void OnCollisionEnter2D(Collision2D collision)

    {
        if (collision.contacts[0].normal.y > 0.5f) isGrounded = true;
    }

#endregion
}