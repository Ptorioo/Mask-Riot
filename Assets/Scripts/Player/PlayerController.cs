#region

using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

#endregion

public class PlayerController : MonoBehaviour
{
#region Public Variables

    public Faction Faction { get; private set; } = Faction.PlayerCharacter;

#endregion

#region Private Variables

    private int            hp;
    private bool           isDead;
    private float          nextAttackTime;
    private Rigidbody2D    rb;
    private SpriteRenderer myRenderer; // (NEW) We cache this to change colors efficiently
    private bool           isGrounded;
    private float          faceDirection;

    [Header("Attack")]
    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private Blade blade;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private SpriteRenderer mask;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    private PlayerCharacterData data;

#endregion

#region Unity events

    private void Awake()
    {
        Assert.IsNotNull(mask , "mask should not be null.");

        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb , "Error , rigidbody2d should in this GameObject!");
    }

    private void Start()
    {
        hp = data.hp;
        blade.EnableSameFactionAttackable();
    }

    private void Update()
    {
        if (isDead) return;
        HandleFaceDirection();
        HandleMoveVelocity();
        HandleJumpAction();
        HandleAttackAction();
    }

#endregion

#region Public Methods

    public void EquipMask(Faction newFaction , Sprite maskSprite)
    {
        Faction      = newFaction;
        mask.enabled = true;
        mask.sprite  = maskSprite;
    }

    public void TakeDamage(int damageValue)
    {
        hp -= damageValue; //damage
        healthBar.UpdateHealthBar(hp , data.hp);
        if (hp <= 0)
        {
            hp = 0;
            Die();
            return;
        }

        StopCoroutine(DamageEffect());
        StartCoroutine(DamageEffect());
    }

#endregion

#region Private Methods

    private IEnumerator DamageEffect()
    {
        body.color = new Color(1 , 0 , 0 , 1);
        yield return new WaitForSeconds(.2f);
        body.color = new Color(1 , 1 , 1 , 1);
    }

    [ContextMenu(nameof(Die))]
    private void Die()
    {
        if (data.isInvincible) return;
        if (isDead) return;
        DieEventHandler?.Invoke(this , EventArgs.Empty);
        rb.linearVelocity = new Vector2(0 , rb.linearVelocity.y);

        isDead = true;
        healthBar.gameObject.SetActive(false);

        // --- fade only this object + Atk subtree ---
        var fadeTargets = new List<SpriteRenderer>();

        // 1. this object's renderer (if any)
        if (TryGetComponent<SpriteRenderer>(out var selfRenderer)) fadeTargets.Add(selfRenderer);

        // 2. Atk and its children
        var atk = transform.Find("Blade");
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

    private void HandleAttackAction()
    {
        if (!Keyboard.current.cKey.wasPressedThisFrame || !(Time.time >= nextAttackTime)) return;
        nextAttackTime = Time.time + attackCooldown;
        blade.StartAttack(Faction , data.atk);
    }

    /// <summary>
    ///     處理人物面向
    /// </summary>
    private void HandleFaceDirection()
    {
        var left  = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        var right = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        if (left)
        {
            faceDirection             = -1f;
            body.transform.localScale = new Vector2(-1 , 1);
        }
        else if (right)
        {
            faceDirection             = 1f;
            body.transform.localScale = Vector2.one;
        }
        else
        {
            faceDirection = 0;
        }
    }

    private void HandleJumpAction()
    {
        if (!isGrounded || !Keyboard.current.spaceKey.wasPressedThisFrame) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x , data.jumpForce);
        isGrounded        = false;
    }

    private void HandleMoveVelocity()
    {
        rb.linearVelocity = new Vector2(faceDirection * data.moveSpeed , rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) isGrounded = true;
    }

#endregion

    public event EventHandler DieEventHandler;
}