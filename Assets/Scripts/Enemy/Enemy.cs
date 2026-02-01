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

    public enum Enemystate
    {
        Rise ,
        Move ,
        Attack ,
        AtkCoolDown ,
        gettingDmg ,
        Dying ,
    }

    public Faction faction => fac;

#endregion

#region Private Variables

    private PlayerHorizontalMovement player;
    private GameObject               target;
    private SpriteRenderer           renderer;
    private int                      dir = 1;
    private float                    timer;
    private bool                     isActive;
    private bool                     haveAtk = false;
    private Enemystate               state;

    [SerializeField]
    private GameObject maskObj;

    [SerializeField]
    private Collider2D body;

    [SerializeField]
    private Collider2D col2D;

    [SerializeField]
    private float modelScale = 2f;

    [SerializeField]
    private Faction fac;

    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    private int attackValue = 0;

    [SerializeField]
    private float distanceRange;

    [SerializeField]
    private float cooldown = 1f;

    [SerializeField]
    private int initHp;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private int hp;

    [SerializeField]
    private Animator atkAnim;

#endregion

#region Unity events

    public void Start()
    {
        hp       = initHp;
        isActive = true;
        InitTargetPLayer();
        player = FindAnyObjectByType<PlayerHorizontalMovement>();

        // REVERTED: Back to 'renderer' (ignores the warning)
        renderer = GetComponent<SpriteRenderer>();

        state = Enemystate.Move;

        float tall = Random.Range(0 , 16);
        body.offset = new Vector2(0 , tall / 100f);

        // FIX: Set size immediately using the new variable
        transform.localScale = new Vector3(modelScale , modelScale , 1f);
    }

#endregion

#region Public Methods

    public void AtkCD()
    {
        timer = 0;
        StartCoroutine(TimerCounter());
    }

    public void Attack()
    {
        Transform target = ScanForEnemies();
        if (target == null) return;

        AnimatorStateInfo tempClipInfo  = atkAnim.GetCurrentAnimatorStateInfo(0);
        float             totalANimTime = tempClipInfo.length;
        float             dist          = Vector3.Distance(target.transform.position , transform.position);

        if (dist >= distanceRange)
        {
            //state = Enemystate.Move;
        }

        if (timer >= totalANimTime)
        {
            timer = 0;
            state = Enemystate.AtkCoolDown;
            return;
        }
        else if (timer <= 0)
        {
            atkAnim.Play("Attack" , 0 , 0);
        }
        else if (timer >= (1f / 60f) * 19f && timer < (1f / 60f) * 24f) //19 - 24th frame
        {
            GetOtherCol();
        }

        timer += Time.fixedDeltaTime;
    }

    public void Die()
    {
        isActive = false;

        DieEventHandler.Invoke(this , EventArgs.Empty);

        // --- mask logic unchanged ---
        if (maskObj != null)
        {
            var tempMask = maskObj;
            tempMask.SetActive(true);
            tempMask.transform.SetParent(null , true);          // keep world transform
            tempMask.transform.position   = transform.position; // use world position
            tempMask.transform.localScale = new Vector3(2f , 2f , 1f);
            if (Random.value < 0.5f)
            {
                Destroy(tempMask);
            }
        }

        healthBar.gameObject.SetActive(false);

        // --- fade only this object + Atk subtree ---
        List<SpriteRenderer> fadeTargets = new List<SpriteRenderer>();

        // 1. this object's renderer (if any)
        if (TryGetComponent<SpriteRenderer>(out var selfRenderer)) fadeTargets.Add(selfRenderer);

        // 2. Atk and its children
        Transform atk = transform.Find("Atk");
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

        // destroy after fade
        DOVirtual.DelayedCall(1f , () =>
                                   {
                                       Destroy(gameObject);
                                   });
    }

    public void GetDamage(int damageValue)
    {
        hp -= damageValue; //damage
        healthBar.UpdateHealthBar(hp , initHp);
        if (hp <= 0)
        {
            hp    = 0;
            state = Enemystate.Dying;
            return;
        }

        StartCoroutine(DamageEffect());
    }

    public void GetOtherCol()
    {
        if (!haveAtk)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;

            Collider2D[] results = new Collider2D[100];

            int    count   = col2D.Overlap(filter , results);
            string tempStr = "obj: ";
            for (int i = 0 ; i < count ; i++)
            {
                PlayerHorizontalMovement playerr;
                Enemy                    enemy = null;
                if (results[i].gameObject.TryGetComponent(out playerr)
                 || (results[i].gameObject.TryGetComponent(out enemy) && enemy != this))
                {
                    if (playerr != null)
                    {
                        tempStr += $"{playerr.gameObject.name},";
                        if (playerr.faction != fac) playerr.GetDamage(attackValue);
                    }

                    if (enemy != null)
                    {
                        if (enemy.fac != fac) enemy.GetDamage(attackValue);
                    }
                }
                else if (results[i].TryGetComponent(out Enemy enemyyy) && enemyyy != this)
                {
                    if (enemyyy.fac != fac) enemyyy.GetDamage(attackValue);
                }
            }

            haveAtk = true;
        }
    }

    public void InitTargetPLayer()
    {
        if (target == null) target = GameObject.Find("Player");
    }

    /// <summary>
    /// When player change mask, Scan the area
    /// </summary>
    /// <returns></returns>
    public Transform ScanForEnemies()
    {
        // 敵人與玩家不同陣營則以玩家為目標
        if (player.faction != faction) return player.transform;

        // 假設使用 Physics.OverlapSphere 獲取周圍物件
        var colliders = Physics2D.OverlapCircleAll(transform.position , 100f);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Enemy>(out var other) && other.faction != faction)
            {
                return other.transform;
            }
        }

        return null;
    }

    public void Statement()
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
                    Move(distanceRange);
                    break;
                case Enemystate.AtkCoolDown :
                    AtkCD();
                    break;
                case Enemystate.gettingDmg : break;
                case Enemystate.Dying :
                    Die();
                    break;
            }
        }
    }

#endregion

#region Private Methods

    private IEnumerator DamageEffect()
    {
        renderer.color = new Color(1 , 0 , 0 , 1);
        yield return new WaitForSeconds(.2f);
        renderer.color = new Color(1 , 1 , 1 , 1);
    }

    private void FixedUpdate()
    {
        Statement();
    }

    [ContextMenu("Hurt1Hp")]
    void Hurt1Hp()
    {
        GetDamage(1);
    }

    private void Move(float distanceRange)
    {
        Transform targetTransform = ScanForEnemies();

        // FIX: If no target found, STOP. Don't run the next lines.
        if (targetTransform == null)
        {
            return;
        }

        bool LR = targetTransform.position.x < transform.position.x;
        dir = LR ? -1 : 1;

        // FIX: Use modelScale variable here
        transform.localScale = new Vector3(dir * modelScale , modelScale , 1);

        transform.Translate(new Vector3(speed * dir * Time.fixedDeltaTime , 0 , 0));
        float dist = Vector3.Distance(targetTransform.position , transform.position);
        if (dist < distanceRange)
        {
            state = Enemystate.Attack;
        }
    }

    private IEnumerator TimerCounter()
    {
        while (timer < cooldown && haveAtk)
        {
            timer += Time.fixedDeltaTime;
            yield return null;
            if (timer > cooldown)
            {
                haveAtk = false;
                timer   = 0f;
                state   = Enemystate.Move;
            }
        }
    }

#endregion

    public static event EventHandler<EventArgs> DieEventHandler;
}