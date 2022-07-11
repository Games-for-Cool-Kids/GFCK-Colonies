using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MilitaryUnit : MonoBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;

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
        onDeSelected?.Invoke();
    }
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
}
