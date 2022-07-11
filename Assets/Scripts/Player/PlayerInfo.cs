using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private List<MilitaryUnit> myUnits = new List<MilitaryUnit>();

    public List<MilitaryUnit> GetMyMilitaryUnits()
    {
        return myUnits;
    }

    // Start is called before the first frame update
    void Start()
    {
        MilitaryUnit.OnUnitSpawned += HandleUnitSpawned;
        MilitaryUnit.OnUnitDespawned += HandleUnitDespawned;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleUnitSpawned(MilitaryUnit unit)
    {
        myUnits.Add(unit);
    }
    private void HandleUnitDespawned(MilitaryUnit unit)
    {
        myUnits.Remove(unit);
    }

}
