using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    private PlayerHorizontalMovement player;
    public Enemystate state;
    public Faction faction;
    public int enemyHp;
    public float speed = 0f;
    public float attackValue = 0f;
    public int dir = 1;
    public float distanceRange;

    public GameObject maskObj;

    public SpriteRenderer renderer;
    private bool isActive;
    public void Start()
    {
        isActive = true;
        player = FindAnyObjectByType<PlayerHorizontalMovement>();
        renderer = GetComponent<SpriteRenderer>();
        state = Enemystate.Move;
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
                case Enemystate.Rise:
                    break;
                case Enemystate.Attack:
                    Attack();
                    break;
                case Enemystate.Move:
                    Move(distanceRange);
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
        transform.localScale = new Vector3(dir, 1, 1);

        transform.Translate(new Vector3(speed * dir * Time.fixedDeltaTime, 0, 0));
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < distanceRange)
        {
            state = Enemystate.Attack;
        }
    }
    public void Attack()
    {
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist >= distanceRange)
        {
            state = Enemystate.Move;
        }
    }
    public void GetDamage(int damageValue)
    {
        enemyHp -= damageValue; //damage
        Debug.Log($"left HP: {enemyHp}");
        if (enemyHp <= 0)
        {
            enemyHp = 0;
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
        GameObject tempMask = Instantiate(maskObj, transform.position, Quaternion.identity);
        tempMask.transform.SetParent(null);
        
        renderer.DOColor(new Color(1, 1, 1, 0), 3f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        transform.DOLocalRotateQuaternion(new Quaternion(0f, 0f, -90f, 0f), 1f);
    }
    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
        gettingDmg,
        Dying,
    }
}
