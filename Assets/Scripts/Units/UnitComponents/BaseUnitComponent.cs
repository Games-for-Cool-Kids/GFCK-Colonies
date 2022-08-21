using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseUnitComponent : MonoBehaviour
{
    public Unit Unit = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Unit = GetComponent<Unit>();
        Debug.Assert(Unit != null);
    }
}
