using DefaultNamespace;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    private PlayerHorizontalMovement player;
    private SpriteRenderer renderer;
    [SerializeField] private GameObject maskObj;
    [SerializeField] private Collider2D body;
    [SerializeField] private Collider2D col2D;

    public Faction faction => faction;
    [SerializeField] private Faction fac;
    [SerializeField] private float speed = 0f;
    [SerializeField] private int attackValue = 0;
    [SerializeField] private float distanceRange;
    [SerializeField] private float cooldown = 1f;
    private int dir = 1;
    private float timer;
    private bool isActive;
    private bool haveAtk = false;
    private Enemystate state;
    [SerializeField] private int initHp;
    [SerializeField]
    private HealthBar healthBar;
    [SerializeField]
    private int hp;

    [SerializeField] private Animator atkAnim;

    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
        AtkCoolDown,
        gettingDmg,
        Dying,
    }
    public void Start()
    {
        hp = initHp;
        isActive = true;
        player = FindAnyObjectByType<PlayerHorizontalMovement>();
        renderer = GetComponent<SpriteRenderer>();
        state = Enemystate.Move;
        float tall = Random.Range(0, 16);
        body.offset = new Vector2(0, tall / 100f);
    }

    private void FixedUpdate()
    {
        Statement();
    }

    public void Statement()
    {
        if (isActive)
        {
            switch (state)
            {
                case Enemystate.Rise: break;
                case Enemystate.Attack:
                    Attack();
                    break;
                case Enemystate.Move:
                    Move(distanceRange);
                    break;
                case Enemystate.AtkCoolDown:
                    AtkCD();
                    break;
                case Enemystate.gettingDmg:
                    break;
                case Enemystate.Dying:
                    Die();
                    break;
            }
        }
    }

    private void Move(float distanceRange)
    {
        bool LR = player.transform.position.x < transform.position.x;
        dir = LR ? -1 : 1;
        int scale = 2;
        transform.localScale = new Vector3(dir*scale, scale, 1);

        transform.Translate(new Vector3(speed * dir * Time.fixedDeltaTime, 0, 0));
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < distanceRange)
        {
            state = Enemystate.Attack;
        }
    }

    public void Attack()
    {
        AnimatorStateInfo tempClipInfo = atkAnim.GetCurrentAnimatorStateInfo(0);
        float totalANimTime = tempClipInfo.length;
        float dist = Vector3.Distance(player.transform.position, transform.position);
        Debug.Log(totalANimTime);

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
            atkAnim.Play("Attack", 0, 0);
        }
        else if (timer >= (1f / 60f) * 19f && timer < (1f / 60f) * 24f) //19 - 24th frame
        {
            Debug.Log("test");
            GetOtherCol();
        }

        timer += Time.fixedDeltaTime;
    }

    [ContextMenu("Hurt1Hp")]
    void Hurt1Hp()
    {
        GetDamage(1);
    }

    public void GetDamage(int damageValue)
    {
        hp -= damageValue; //damage
        healthBar.UpdateHealthBar(hp, initHp);
        Debug.Log($"left HP: {hp}");
        if (hp <= 0)
        {
            hp = 0;
            state = Enemystate.Dying;
            return;
        }

        StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        renderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(.2f);
        renderer.color = new Color(1, 1, 1, 1);
    }

    public void Die()
    {
        isActive = false;
        GameObject tempMask = maskObj;

        tempMask.SetActive(true);
        tempMask.transform.SetParent(null, false);
        tempMask.transform.localPosition = transform.position;

        healthBar.gameObject.SetActive(false);
        renderer.DOColor(new Color(1, 1, 1, 0), 3f)
                .OnComplete(() =>
                            {
                                Destroy(gameObject);
                            });
        transform.DOLocalRotateQuaternion(new Quaternion(0f, 0f, -90f, 0f), 1f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerHorizontalMovement>(out PlayerHorizontalMovement pplayer))
        {
            Debug.Log($"hurt {pplayer.gameObject.name}!");
        }
        else if (collision.TryGetComponent<Enemy>(out Enemy com) && com != this)
        {
            //can not body hit
        }
    }
    public void GetOtherCol()
    {
        if (!haveAtk)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;

            Collider2D[] results = new Collider2D[100];

            int count = col2D.Overlap(filter, results);
            string tempStr = "obj: ";
            for (int i = 0; i < count; i++)
            {
                Debug.Log("������G" + results[i].gameObject.name);
                PlayerHorizontalMovement playerr = null;
                Enemy enemy = null;
                if (results[i].gameObject.TryGetComponent<PlayerHorizontalMovement>(out playerr)
                    || (results[i].gameObject.TryGetComponent<Enemy>(out enemy) && enemy != this))
                {
                    if (playerr != null)
                    {
                        Debug.Log("������G" + playerr.gameObject.name);
                        tempStr += $"{playerr.gameObject.name},";
                        if (playerr.faction != fac)
                            playerr.GetDamage(attackValue);
                    }
                    if (enemy != null)
                    {
                        Debug.Log("������ĤH���ĤH�G" + enemy.gameObject.name);
                        if (enemy.fac != fac)
                            enemy.GetDamage(attackValue);
                    }
                }
                else if (results[i].TryGetComponent<Enemy>(out Enemy enemyyy) && enemyyy != this)
                {
                    if (enemyyy.fac != fac)
                        enemyyy.GetDamage(attackValue);
                }
            }
            Debug.Log(tempStr);
            haveAtk = true;
        }
    }
    public void AtkCD()
    {
        timer = 0;
        StartCoroutine(TimerCounter());
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
                timer = 0f;
                state = Enemystate.Move;
            }

        }
    }
}