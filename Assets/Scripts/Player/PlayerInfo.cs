using UnityEngine;

public class PlayerInfo : MonoBehaviourSingleton<PlayerInfo>
{
    public Faction playerFaction = new Faction(); // Later on this might be a list of factions. For now there's just one.

    void Start()
    {
        Unit.OnUnitSpawned += AddUnit;
        Unit.OnUnitDespawned += RemoveUnit;
    }

    private void AddUnit(GameObject unit)
    {
        // TODO Will need some different component, probably related with military
        if(unit.GetComponent<ComponentSelect>())
        {
            playerFaction.MilitaryUnits.Add(unit);
        }     
    }
    private void RemoveUnit(GameObject unit)
    {
        if(playerFaction.MilitaryUnits.Contains(unit))
        {
            Debug.Assert(unit.GetComponent<ComponentSelect>());

            playerFaction.MilitaryUnits.Remove(unit);
        } 
    }
}
