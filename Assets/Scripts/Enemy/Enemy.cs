#region

using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using Faction_1 = Faction;

#endregion

public class Enemy : MonoBehaviour
{
#region Public Variables

    public bool IsDead { get; private set; }

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
    private Enemystate       state;

    private readonly float detectTargetRange = 3f;

    private int    hp;
    private double timer;
    private float  moveSpeed;
    private float  attackCooldown;

    private Transform target;

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
        Faction        = data.faction;
        player         = FindFirstObjectByType<PlayerController>();
        state          = Enemystate.Move;
        healthBar.Hide();
    }

    private void Update()
    {
        Statement();
    }

#endregion

#region Public Methods

    public void TakeDamage(int damageValue)
    {
        if (IsDead) return;
        hp -= damageValue; //damage
        if (hp <= 0)
        {
            hp    = 0;
            state = Enemystate.Dying;
            Die();
            return;
        }

        healthBar.UpdateHealthBar(hp , data.hp);
        StartCoroutine(DamageEffect());
    }

#endregion

#region Private Methods

    private void Attack()
    {
        blade.StartAttack(Faction , data.atk);
        if (blade.IsAttackEnded() == false)
        {
            state = Enemystate.AtkCoolDown;
            timer = 0;
            StartCoroutine(AttackCooldownTimer());
        }
    }

    private IEnumerator AttackCooldownTimer()
    {
        while (timer < attackCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
            if (timer >= attackCooldown)
            {
                FindTarget();
                if (target == null)
                {
                    state = Enemystate.Move;
                    break;
                }

                state = GetDistanceWithTarget() > detectTargetRange ? Enemystate.Move : Enemystate.Attack;
            }
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
        IsDead = true;
        DieEventHandler?.Invoke(this , EventArgs.Empty);
        DropMask();
        healthBar.Hide();

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

    [ContextMenu(nameof(DropMask))]
    private void DropMask()
    {
        maskObj.gameObject.SetActive(true);
        maskObj.transform.SetParent(null , true); // keep world transform
        maskObj.SetFaction(Faction , data.maskSprite);
        // 隨機掉面具，50% 掉落
        // if (Random.value < 0.5f) Destroy(maskObj.gameObject);
    }

    private void FindTarget()
    {
        if (player.Faction != Faction && target != player.transform && player.IsDead == false)
        {
            target = player.transform;
        }
        else if (target == null)
        {
            target = ScanForTarget();
        }
        else if (target != null)
        {
            var targetIsPlayer                = target == player.transform;
            var targetIsPlayAndSameFaction    = targetIsPlayer && player.Faction == Faction;
            var targetIsPlayerAndPlayerIsDead = targetIsPlayer && player.IsDead;
            var targetIsEnemyAndIsDead        = target.TryGetComponent(out Enemy enemy) && enemy.IsDead;

            if (targetIsPlayAndSameFaction || targetIsEnemyAndIsDead || targetIsPlayerAndPlayerIsDead) target = ScanForTarget();
        }
    }

    private float GetDistanceWithTarget()
    {
        Vector2 targetPos = target.transform.position;
        Vector2 myPos     = transform.position;
        targetPos.y = 0;
        myPos.y     = 0;
        var dist = Vector2.Distance(targetPos , myPos);
        return dist;
    }

    [ContextMenu("Hurt1Hp")]
    void Hurt1Hp()
    {
        TakeDamage(1);
    }

    private void Move()
    {
        FindTarget();

        // FIX: If no target found, STOP. Don't run the next lines.
        if (target == null) return;

        var onLeft = target.position.x < transform.position.x;
        dir = onLeft ? -1 : 1;

        body.transform.localScale = onLeft ? new Vector2(-1 , 1) : Vector2.one;

        transform.Translate(new Vector3(moveSpeed * dir * Time.deltaTime , 0 , 0));
        if (GetDistanceWithTarget() <= detectTargetRange)
        {
            state = Enemystate.Attack;
        }
    }

    private void OnValidate()
    {
        if (body.sprite != data.bodySprite) body.sprite = data.bodySprite;
    }

    /// <summary>
    /// When player change mask, Scan the area
    /// </summary>
    /// <returns></returns>
    private Transform ScanForTarget()
    {
        // 假設使用 Physics.OverlapSphere 獲取周圍物件
        var colliders = Physics2D.OverlapCircleAll(transform.position , 100f);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Enemy>(out var otherEnemy) && otherEnemy.Faction != Faction && otherEnemy.IsDead == false)
            {
                return otherEnemy.transform;
            }
        }

        return null;
    }

    private void Statement()
    {
        if (IsDead) return;
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
            case Enemystate.Dying :       break;
            default :                     throw new ArgumentOutOfRangeException($"does not handle this state: [{state}]");
        }
    }

#endregion

    public static event EventHandler DieEventHandler;
}