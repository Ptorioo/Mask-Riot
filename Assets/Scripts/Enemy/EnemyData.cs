#region

using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "EnemyData" , menuName = "EnemyData" , order = 0)]
public class EnemyData : ScriptableObject
{
#region Public Variables

    public Faction faction        = Faction.Skeleton;
    public float   attackCooldown = 2;
    public float   moveSpeed      = 2;
    public int     atk            = 1;
    public int     hp             = 5;
    public Sprite  bodySprite;
    public Sprite  bodySpriteWithNoMask;
    public Sprite  maskSprite;

#endregion
}