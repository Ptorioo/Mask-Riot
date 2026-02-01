using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mask : MonoBehaviour
{
    public Faction faction;

    // How close do you need to be? (1.5 units is usually good)
    [SerializeField]
    private float pickupRange = 1.5f;

    [SerializeField]
    private GameObject hint;

    private void Start()
    {
        hint.SetActive(false);
    }

    private void Update()
    {
        // 1. Safety Check: Does the player exist?
        if (PlayerHorizontalMovement.Instance == null) return;

        // 2. MATH CHECK: Calculate distance directly
        float distance = Vector2.Distance(transform.position , PlayerHorizontalMovement.Instance.transform.position);

        // 3. Logic: If close enough AND pressing E
        if (distance <= pickupRange)
        {
            hint.SetActive(true);
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                PerformPickup();
            }
        }
        else
        {
            hint.SetActive(false);
        }
    }

    private void PerformPickup()
    {
        if (faction == Faction.PlayerCharacter)
        {
            Debug.LogWarning("Mask Faction is Default! Ignoring.");
            return;
        }

        // CHANGE: Use GetComponentInChildren to find the sprite even if it's on a sub-object
        var renderer = GetComponent<SpriteRenderer>();
        PlayerHorizontalMovement.Instance.EquipMask(faction , renderer.sprite);
        Destroy(gameObject);
    }

    // Optional: Draw a circle in the Editor so you can see the range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position , pickupRange);
    }
}