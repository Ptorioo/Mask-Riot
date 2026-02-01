#region

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

#endregion

public class Mask : MonoBehaviour
{
#region Private Variables

    private Faction faction = Faction.Skeleton;

    [SerializeField]
    private GameObject hint;

#endregion

#region Unity events

    private void Start()
    {
        hint.SetActive(false);
    }

    private void Update()
    {
        // hint shows.
        if (!hint.activeSelf) return;
        if (Keyboard.current.eKey.wasPressedThisFrame) PerformPickup();
    }

#endregion

#region Public Methods

    public void SetFaction(Faction fac)
    {
        faction = fac;
    }

#endregion

#region Private Methods

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlayerController _)) hint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlayerController _))
        {
            hint.SetActive(false);
        }
    }

    private void PerformPickup()
    {
        Assert.IsTrue(faction != Faction.PlayerCharacter);
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var player         = FindFirstObjectByType<PlayerController>();
        player.EquipMask(faction , spriteRenderer.sprite);
        Destroy(gameObject);
    }

#endregion
}