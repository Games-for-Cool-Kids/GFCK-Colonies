using System;
using UnityEngine;
using UnityEngine.Events;

public class MilitaryUnit : Unit
{
    public static event Action<GameObject> OnUnitSpawned;
    public static event Action<GameObject> OnUnitDespawned;

    private bool invoked = false;


    protected new void Update()
    {
        if (!invoked)
        {
            OnUnitSpawned?.Invoke(gameObject);
            invoked = true;
        }

        base.Update();
    }

    void OnDestroy()
    {
        OnUnitDespawned?.Invoke(gameObject);
    }
}
