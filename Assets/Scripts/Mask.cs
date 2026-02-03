#region

using UnityEngine;

#endregion

public class Mask : MonoBehaviour
{
#region Public Variables

    public Faction Faction => faction;
    public Sprite  Sprite  => body.sprite;

#endregion

#region Private Variables

    [SerializeField]
    private Faction faction = Faction.Skeleton;

    [SerializeField]
    private GameObject hint;

    [SerializeField]
    private SpriteRenderer body;

#endregion

#region Unity events

    private void Start()
    {
        hint.SetActive(false);
    }

#endregion

#region Public Methods

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void HideHint()
    {
        hint.SetActive(false);
    }

    public void SetFaction(Faction fac , Sprite maskSprite)
    {
        faction     = fac;
        body.sprite = maskSprite;
    }

    public void ShowHint()
    {
        hint.SetActive(true);
    }

#endregion
}