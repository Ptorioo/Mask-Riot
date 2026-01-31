using UnityEngine;

public class Enemy : MonoBehaviour
{
    //public AttackArea;
    public float speed = 0f;
    public float attackValue = 0f;
    public int dir = 1;
    private Rigidbody2D rigg2D;
    public Enemystate state;
    public PlayerHorizontalMovement player;
    public void Start()
    {
        rigg2D = GetComponent<Rigidbody2D>();
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

                break;
            case Enemystate.Move:
                Move();
                break;
        }
    }
    private void Move(/*玩家或是對應的Transform*/)
    {
        dir = player.transform.position.x < transform.position.x ? -1 : 1;

        rigg2D.AddForce(new Vector2(dir * speed * Time.fixedDeltaTime, transform.position.y));
    }
    public enum Enemystate
    {
        Rise,
        Move,
        Attack,
    }
}
