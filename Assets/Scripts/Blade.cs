#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class Blade : MonoBehaviour
{
#region Private Variables

    private bool attackStarted;

    private readonly Collider2D[] results = new Collider2D[100];

    private bool    hitboxStarted;
    private Faction faction;
    private int     damage;

    private readonly List<GameObject> attackedTargetsFromSameAttack = new List<GameObject>();

    private bool inAttackingAnimState;

    [SerializeField]
    private Collider2D hitBox;

    [SerializeField]
    private Animator animator;

#endregion

#region Unity events

    private void Start()
    {
        hitBox.enabled = false;
    }

    private void Update()
    {
        if (hitboxStarted) FilterTargetAndDealDamage(faction , damage);
        ResetAttackingFlag();
    }

#endregion

#region Public Methods

    public bool IsAttackEnded()
    {
        return inAttackingAnimState == false;
    }

    public void StartAttack(Faction selfFaction , int damage)
    {
        if (IsInAttackAnimState()) return;
        inAttackingAnimState = true;
        this.damage          = damage;
        faction              = selfFaction;
        hitBox.enabled       = true;
        animator.Play("Slash");
    }

#endregion

#region Private Methods

    private void FilterTargetAndDealDamage(Faction selfFaction , int damage)
    {
        var filter = new ContactFilter2D { useTriggers = true };
        var count  = hitBox.Overlap(filter , results);
        for (var i = 0 ; i < count ; i++)
        {
            var triggeredGameObject = results[i].gameObject;
            // 同一個攻擊內，不重複攻擊相同目標
            if (attackedTargetsFromSameAttack.Contains(triggeredGameObject)) continue;
            if (triggeredGameObject.TryGetComponent(out PlayerController player))
                if (player.Faction != selfFaction)
                {
                    player.TakeDamage(damage);
                    attackedTargetsFromSameAttack.Add(player.gameObject);
                }

            if (triggeredGameObject.TryGetComponent(out Enemy enemy))
                if (enemy.Faction != selfFaction)
                {
                    enemy.TakeDamage(damage);
                    attackedTargetsFromSameAttack.Add(enemy.gameObject);
                }
        }
    }

    [ContextMenu("Hitbox結束幀")]
    private void HitboxEndFrame()
    {
        hitboxStarted = false;
        attackedTargetsFromSameAttack.Clear();
        hitBox.enabled = false;
    }

    [ContextMenu("Hitbox開始幀")]
    private void HitboxStartFrame()
    {
        hitboxStarted = true;
    }

    private bool IsInAttackAnimState()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Slash");
    }

    private void ResetAttackingFlag()
    {
        if (inAttackingAnimState)
            if (IsInAttackAnimState() == false)
                inAttackingAnimState = false;
    }

    [ContextMenu(nameof(TestAttack))]
    private void TestAttack()
    {
        StartAttack(Faction.Witch , 3);
    }

#endregion
}