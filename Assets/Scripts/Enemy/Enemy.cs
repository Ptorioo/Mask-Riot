using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlayerHorizontalMovement player;
    public Enemystate state;
    public Faction faction;

    public int enemyHp
    {
        get => enemyHp;
        set
        {
            if (value < 0)
                value = 0;
        }
    }
    public float speed = 0f;
    public float attackValue = 0f;
    public int dir = 1;
    public float distanceRange;

    public void Start()
    {
        player = FindAnyObjectByType<PlayerHorizontalMovement>();
        state = Enemystate.Move;
    }
    private void FixedUpdate()
    {
        Statement();
    }
    public void Statement()
    {
        switch (state)
        {
            case Enemystate.Rise:
                //鑽出動畫
                break;
            case Enemystate.Attack:
                Attack();
                break;
            case Enemystate.Move:
                Move(distanceRange);
                break;
            case Enemystate.gettingDmg:

                break;
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
    public void GetDamage(int damage) => GetDamageCorou(damage);
    public IEnumerator GetDamageCorou(int damage)
    {
        enemyHp -= damage;

        if (enemyHp <= 0)
        {
            //死亡邏輯
            yield break;
        }


        yield return new WaitForSeconds(1f);
        //受到傷害邏輯

    }
    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
        gettingDmg,
    }
}
