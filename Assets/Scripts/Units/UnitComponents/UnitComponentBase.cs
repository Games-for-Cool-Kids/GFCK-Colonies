using UnityEngine;


public abstract class UnitComponentBase : MonoBehaviour
{
    public Unit Owner = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Owner = GetComponent<Unit>();
        Debug.Assert(Owner != null);
    }
}
