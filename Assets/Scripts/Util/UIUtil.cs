using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtil
{
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject(); // Confusingly game object means an EventSystem (ui) object.
    }
}
