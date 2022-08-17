using UnityEngine;

public class PlayerInfo : MonoBehaviourSingleton<PlayerInfo>
{
    public Faction playerFaction = new Faction(); // Later on this might be a list of factions. For now there's just one.

    void Start()
    {
        MilitaryUnit.OnUnitSpawned += AddUnit;
        MilitaryUnit.OnUnitDespawned += RemoveUnit;
    }

    private void AddUnit(GameObject unit)
    {
        playerFaction.MilitaryUnits.Add(unit);
    }
    private void RemoveUnit(GameObject unit)
    {
        playerFaction.MilitaryUnits.Remove(unit);
    }
}
