using UnityEngine;

/// <summary>
/// [TagSelector] attribute for a tags dropdown editor for scripts. (Like the one to set a GameObject's tag in inspector.)
/// </summary>
public class TagSelectorAttribute : PropertyAttribute
{
    public bool UseDefaultTagFieldDrawer = false;
}