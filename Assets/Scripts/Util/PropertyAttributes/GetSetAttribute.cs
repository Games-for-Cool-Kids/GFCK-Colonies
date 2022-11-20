using UnityEngine;

/// <summary>
/// [GetSet] attribute for properties in scripts so we can edit them in the inspector. (In Unity normally only fields can be visible/edited in the inspector.)
/// </summary>
public sealed class GetSetAttribute : PropertyAttribute
{
    public readonly string name;
    public bool dirty;

    public GetSetAttribute(string name)
    {
        this.name = name;
    }
}