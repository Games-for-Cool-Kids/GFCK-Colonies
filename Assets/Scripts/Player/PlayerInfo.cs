using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public List<MilitaryUnit> militaryUnits = new List<MilitaryUnit>();

    private static PlayerInfo _instance;
    public static PlayerInfo Instance
    {
        get
        {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MilitaryUnit.OnUnitSpawned += HandleUnitSpawned;
        MilitaryUnit.OnUnitDespawned += HandleUnitDespawned;
    }

    private void HandleUnitSpawned(MilitaryUnit unit)
    {
        militaryUnits.Add(unit);
    }
    private void HandleUnitDespawned(MilitaryUnit unit)
    {
        militaryUnits.Remove(unit);
    }

}
