using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MilitaryUnit : MonoBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;

    public void Select()
    {
        onSelected?.Invoke();
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
