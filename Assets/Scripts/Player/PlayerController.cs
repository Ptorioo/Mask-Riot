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

    public bool IsDead { get; private set; }

    public Faction Faction { get; private set; } = Faction.PlayerCharacter;

#endregion

#region Private Variables

    private int            hp;
    private float          nextAttackTime;
    private Rigidbody2D    rb;
    private SpriteRenderer myRenderer; // (NEW) We cache this to change colors efficiently
    private bool           isGrounded;
    private float          moveDirection;

    private Mask pickableMask;

    private int faceDirection = 1;

    [Header("Attack")]
    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private Blade blade;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private SpriteRenderer maskBody;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    private PlayerCharacterData data;

    [SerializeField]
    private BoxCollider2D hurtBox;

#endregion

#region Unity events

    private void Awake()
    {
        Assert.IsNotNull(maskBody , "mask should not be null.");

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
        if (IsDead) return;
        HandleFaceDirection();
        HandleMoveVelocity();
        HandleJumpAction();
        HandleAttackAction();
        HandlePickMask();
    }

#endregion

#region Public Methods

    public void TakeDamage(int damageValue)
    {
        if (IsDead) return;
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

    private void DetectionContactMask(Collider2D col)
    {
        // 只顯示第一個，之後的面具不顯示
        if (pickableMask != null) return;
        if (!col.TryGetComponent<Mask>(out var mask)) return;
        mask.ShowHint();
        pickableMask = mask;
    }

    [ContextMenu(nameof(Die))]
    private void Die()
    {
        if (data.isInvincible) return;
        if (IsDead) return;
        DieEventHandler?.Invoke(this , EventArgs.Empty);
        // rb.linearVelocity      = new Vector2(0 ,                  rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(-faceDirection * 15 , 15);

        IsDead = true;
        healthBar.Hide();

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
        transform.DOLocalRotate(new Vector3(0 , 0 , 90f) , 1f);
    }

    private void EquipMask(Faction newFaction , Sprite maskSprite)
    {
        Faction          = newFaction;
        maskBody.enabled = true;
        maskBody.sprite  = maskSprite;
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
            moveDirection             = -1f;
            faceDirection             = -1;
            body.transform.localScale = new Vector2(-1 , 1);
        }
        else if (right)
        {
            moveDirection             = 1f;
            faceDirection             = 1;
            body.transform.localScale = Vector2.one;
        }
        else
        {
            moveDirection = 0;
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
        rb.linearVelocity = new Vector2(moveDirection * data.moveSpeed , rb.linearVelocity.y);
    }

    private void HandlePickMask()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame) PerformPickup();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f) isGrounded = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        DetectionContactMask(col);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.TryGetComponent<Mask>(out var mask)) return;
        mask.HideHint();
        pickableMask = null;
    }

    private void PerformPickup()
    {
        if (pickableMask == null) return;
        EquipMask(pickableMask.Faction , pickableMask.Sprite);
        pickableMask.DestroyThis();
        pickableMask = null;
        var hurtboxContactResults = new List<Collider2D>();
        hurtBox.Overlap(hurtboxContactResults);
        hurtboxContactResults.ForEach(DetectionContactMask);
    }

#endregion

    public event EventHandler DieEventHandler;
}