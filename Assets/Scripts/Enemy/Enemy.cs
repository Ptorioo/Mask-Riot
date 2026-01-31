using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PlayerHorizontalMovement player;
    public Enemystate state;
    //public AttackArea;
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
                //Æp¥X°Êµe
                break;
            case Enemystate.Attack:
                Attack();
                break;
            case Enemystate.Move:
                Move(distanceRange);
                break;
        }
    }
    private void Move(float distanceRange)
    {
        bool LR = player.transform.position.x < transform.position.x;
        int dir = this.dir;
        dir = LR ? -1 : 1;
        //transform.localScale = new Vector3(transform.localScale.x, 1, 1);

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
    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
    }
}
