using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitBox : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public LayerMask hitMask = ~0;

    public BoxCollider2D box;

    private float originalOffsetX;

    public PlayerHorizontalMovement playerControl;

    void Awake()
    {
        if (!box) box = GetComponent<BoxCollider2D>();
        originalOffsetX = box.offset.x;
    }

    void LateUpdate()
    {
        // -------- Flip hitbox horizontally --------
        Vector2 offset = box.offset;
        offset.x = playerSprite && playerSprite.flipX ? -originalOffsetX : originalOffsetX;
        box.offset = offset;

        DetectHits();
    }

    void DetectHits()
    {
        // Make sure triggers can be detected if your enemy colliders are triggers (or your hitbox is a trigger)
        // (You can also set this in Project Settings > Physics 2D)
        // Physics2D.queriesHitTriggers = true;

        // World-space center/size from collider bounds (includes offset + scale)
        Vector2 center = box.bounds.center;
        Vector2 size = box.bounds.size;

        // Rotation in degrees (2D uses Z)
        float angle = transform.eulerAngles.z;

        Collider2D[] cols = Physics2D.OverlapBoxAll(center, size, angle, hitMask);

        // Debug.Log($"HitBox hit: {cols.Length}");

        foreach (var col in cols)
        {
            if (!col) continue;

            if (col == box) continue;

            if (transform.parent != null && col.transform == transform.parent) continue;
            if (transform.parent != null && col.transform.IsChildOf(transform.parent) && col.GetComponentInParent<Enemy>() == null)
            {
                // optional: ignore parent's hierarchy if you really meant "parent area"
            }

            var enemy = col.GetComponentInParent<Enemy>();
            if (enemy != null) {
                if(playerControl != null && playerControl.IsAttacking)
                    enemy.GetDamage();
                // Debug.Log(enemy.gameObject.name);
            }
                
        }
    }
}
