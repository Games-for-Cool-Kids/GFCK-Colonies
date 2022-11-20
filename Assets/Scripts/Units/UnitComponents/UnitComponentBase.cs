using UnityEngine;


public abstract class UnitComponentBase : MonoBehaviour
{
    public Unit Owner = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Owner = gameObject.GetComponent<Unit>();
        Debug.Assert(Owner != null);
    }
}
