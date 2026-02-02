#region

using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class Enemy : MonoBehaviour
{
#region Public Variables

    public Faction Faction { get; private set; }

#endregion

#region Private Variables

    private enum Enemystate
    {
        Rise ,
        Move ,
        Attack ,
        AtkCoolDown ,
        gettingDmg ,
        Dying ,
    }

    private PlayerController player;
    private int              dir = 1;
    private bool             isActive;
    private Enemystate       state;

    private readonly float detectTargetRange = 3.5f;

    private int    hp;
    private double timer;
    private float  moveSpeed;
    private float  attackCooldown;

    [SerializeField]
    private Mask maskObj;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    private EnemyData data;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private Blade blade;

#endregion

#region Unity events

    public void Start()
    {
        hp             = data.hp;
        moveSpeed      = data.moveSpeed;
        attackCooldown = data.attackCooldown;
        body.sprite    = data.bodySprite;
        isActive       = true;
        Faction        = data.faction;
        player         = FindFirstObjectByType<PlayerController>();
        state          = Enemystate.Move;
    }

    private void Update()
    {
        Statement();
    }

#endregion

#region Public Methods

    public void TakeDamage(int damageValue)
    {
        hp -= damageValue; //damage
        healthBar.UpdateHealthBar(hp , data.hp);
        if (hp <= 0)
        {
            hp    = 0;
            state = Enemystate.Dying;
            return;
        }

        StartCoroutine(DamageEffect());
    }

#endregion

#region Private Methods

    private void Attack()
    {
        blade.StartAttack(Faction , data.atk);
        // var target = ScanForTarget();
        // if (target == null) return;
        if (blade.IsAttackEnded() == false)
        {
            state = Enemystate.AtkCoolDown;
            timer = 0;
            StartCoroutine(TimerCounter());
        }
    }

    private IEnumerator DamageEffect()
    {
        body.color = new Color(1 , 0 , 0 , 1);
        yield return new WaitForSeconds(.2f);
        body.color = new Color(1 , 1 , 1 , 1);
    }

    [ContextMenu(nameof(Die))]
    private void Die()
    {
        isActive = false;

        DieEventHandler?.Invoke(this , EventArgs.Empty);

        DropMask();

        healthBar.gameObject.SetActive(false);

        // --- fade only this object + Atk subtree ---
        var fadeTargets = new List<SpriteRenderer>();

        // 1. this object's renderer (if any)
        if (TryGetComponent<SpriteRenderer>(out var selfRenderer)) fadeTargets.Add(selfRenderer);

        // 2. Atk and its children
        var atk = transform.Find("Atk");
        if (atk != null) fadeTargets.AddRange(atk.GetComponentsInChildren<SpriteRenderer>());

        // fade
        foreach (var r in fadeTargets)
            r.DOColor(
                    new Color(r.color.r , r.color.g , r.color.b , 0f) ,
                    3f
                    );

        // rotation
        transform.DOLocalRotate(new Vector3(0 , 0 , -90f) , 1f);

        // destroy after fade
        DOVirtual.DelayedCall(1f , () =>
                                   {
                                       Destroy(gameObject);
                                   });
    }

    private void DropMask()
    {
        maskObj.gameObject.SetActive(true);
        maskObj.transform.SetParent(null , true);          // keep world transform
        maskObj.transform.position   = transform.position; // use world position
        maskObj.transform.localScale = new Vector3(2f , 2f , 1f);
        maskObj.SetFaction(Faction);
        // 隨機掉面具，50% 掉落
        if (Random.value < 0.5f) Destroy(maskObj);
    }

    private float GetDistanceWithPlayer()
    {
        var dist = Vector3.Distance(player.transform.position , transform.position);
        return dist;
    }

    [ContextMenu("Hurt1Hp")]
    void Hurt1Hp()
    {
        TakeDamage(1);
    }

    private void Move()
    {
        var targetTransform = ScanForTarget();

        // FIX: If no target found, STOP. Don't run the next lines.
        if (targetTransform == null) return;

        var onLeft = targetTransform.position.x < transform.position.x;
        dir = onLeft ? -1 : 1;

        body.transform.localScale = onLeft ? new Vector2(-1 , 1) : Vector2.one;

        transform.Translate(new Vector3(moveSpeed * dir * Time.deltaTime , 0 , 0));
        if (GetDistanceWithPlayer() <= detectTargetRange)
        {
            state = Enemystate.Attack;
        }
    }

    /// <summary>
    /// When player change mask, Scan the area
    /// </summary>
    /// <returns></returns>
    private Transform ScanForTarget()
    {
        // 敵人與玩家不同陣營則以玩家為目標
        if (player.Faction != Faction) return player.transform;

        // 假設使用 Physics.OverlapSphere 獲取周圍物件
        var colliders = Physics2D.OverlapCircleAll(transform.position , 100f);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Enemy>(out var other) && other.Faction != Faction)
            {
                return other.transform;
            }
        }

        return null;
    }

    private void Statement()
    {
        if (isActive)
        {
            switch (state)
            {
                case Enemystate.Rise : break;
                case Enemystate.Attack :
                    Attack();
                    break;
                case Enemystate.Move :
                    Move();
                    break;
                case Enemystate.AtkCoolDown : break;
                case Enemystate.gettingDmg :  break;
                case Enemystate.Dying :
                    Die();
                    break;
            }
        }
    }

    private IEnumerator TimerCounter()
    {
        while (timer < attackCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
            if (timer >= attackCooldown)
            {
                state = GetDistanceWithPlayer() > detectTargetRange ? Enemystate.Move : Enemystate.Attack;
            }
        }
    }

#endregion

    public static event EventHandler DieEventHandler;
}