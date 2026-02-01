using UnityEngine;

public class FactionMember : MonoBehaviour
{
    [SerializeField]
    private Faction faction;

    public Faction Faction => faction;

    public void SetFaction(Faction newFaction)
    {
        faction = newFaction;
    }

    public bool IsFaction(Faction f)
    {
        return faction == f;
    }
}
