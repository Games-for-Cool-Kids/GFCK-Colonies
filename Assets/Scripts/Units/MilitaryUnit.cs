using System;
using UnityEngine;
using UnityEngine.Events;

public class MilitaryUnit : Unit
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

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

    public void Select()
    {
        onSelected?.Invoke();
    }

    void OnDestroy()
    {
        OnUnitDespawned?.Invoke(gameObject);
    }

    public void Deselect()
    {
        onDeselected?.Invoke();
    }
}
