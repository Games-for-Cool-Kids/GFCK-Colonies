using Economy;
using UnityEngine;

public class PlayerInfo : MonoBehaviourSingleton<PlayerInfo>
{
    public Faction playerFaction = new Faction(); // Later on this might be a list of factions. For now there's just one.

    public ResourceTransferRequestsManager ResourceTransferRequestManager = new();

    void Start()
    {
        Unit.OnUnitSpawned += AddUnit;
        Unit.OnUnitDespawned += RemoveUnit;
    }

    private void AddUnit(GameObject unit)
    {
        if(playerFaction.Population.Count >= playerFaction.MaxPopulation)
        {
            Destroy(unit);
            Debug.LogError("Trying to spawn more units than population cap allows!");
            return;
        }

        playerFaction.Population.Add(unit);

        // TODO Will need some different component, probably related with military
        if (unit.GetComponent<UnitComponentSelect>())
        {
            playerFaction.MilitaryUnits.Add(unit);
        }
    }
    private void RemoveUnit(GameObject unit)
    {
        playerFaction.Population.Remove(unit);

        if (playerFaction.MilitaryUnits.Contains(unit))
        {
            Debug.Assert(unit.GetComponent<UnitComponentSelect>());

            playerFaction.MilitaryUnits.Remove(unit);
        }
    }
}
