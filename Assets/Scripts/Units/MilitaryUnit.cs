using System;
using UnityEngine;
using UnityEngine.Events;

public class MilitaryUnit : Unit
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<MilitaryUnit> OnUnitSpawned;
    public static event Action<MilitaryUnit> OnUnitDespawned;

    private bool invoked = false;
   
    private void Start()
    {
    }

    private void Update()
    {
        if (!invoked)
        {
            OnUnitSpawned?.Invoke(this);
            invoked = true;
        }
    }

    public void Select()
    {
        onSelected?.Invoke();
    }

    void OnDestroy()
    {
        OnUnitDespawned?.Invoke(this);
    }

    public void Deselect()
    {
        onDeselected?.Invoke();
    }
}
