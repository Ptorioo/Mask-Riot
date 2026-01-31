using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void Start()
    {
        enemyHp = 10;
        player = FindAnyObjectByType<PlayerHorizontalMovement>();
        renderer = GetComponent<SpriteRenderer>();
        state = Enemystate.Move;
        GetDamage();
        Debug.Log($"現在HP剩餘: {enemyHp}");
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
    [ContextMenu("扣血")]
    public void GetDamage() => StartCoroutine(GetDamageCorou());
    public IEnumerator GetDamageCorou()
    {
        enemyHp -= 1; //damage
        Debug.Log($"現在HP剩餘: {enemyHp}");
        if (enemyHp <= 0)
        {
            //死亡邏輯
            enemyHp = 0;
            Die();
            yield break;
        }
        renderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(.1f);
        renderer.color = new Color(1, 1, 1, 1);

        //受到傷害邏輯
    }
    [ContextMenu("死")]
    public void Die()
    {
        GameObject tempMask = Instantiate(maskObj, transform.position, Quaternion.identity);
        tempMask.transform.SetParent(null);
        Destroy(gameObject);
    }
    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
        gettingDmg,
    }
}
