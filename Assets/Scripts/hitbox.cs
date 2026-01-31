using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitBox : MonoBehaviour
{
    public SpriteRenderer playerSprite;

    public LayerMask hitMask = ~0; // layers to detect (default: everything)

    private BoxCollider2D box;
    private float originalOffsetX;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        originalOffsetX = box.offset.x;
    }

    void LateUpdate()
    {
        // -------- Flip hitbox horizontally --------
        Vector2 offset = box.offset;
        offset.x = playerSprite.flipX ? -originalOffsetX : originalOffsetX;
        box.offset = offset;

        // -------- BoxCastAll detection --------
        DetectHits();
    }

    void DetectHits()
    {
        // World-space center of the hitbox
        Vector2 center = transform.TransformPoint(box.offset);

        // World-space size (respect scale)
        Vector2 size = Vector2.Scale(box.size, transform.lossyScale);

        // Rotation in degrees (2D uses Z)
        float angle = transform.eulerAngles.z;

        // BoxCastAll with zero distance = overlap test
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            center,
            size,
            angle,
            Vector2.zero,
            0f,
            hitMask
        );

        foreach (var hit in hits)
        {
            // Ignore self
            if (hit.collider == box)
                continue;

            Debug.Log(
                $"HitBox hit: {hit.collider.name} | " +
                $"Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}"
            );
        }
    }
}
