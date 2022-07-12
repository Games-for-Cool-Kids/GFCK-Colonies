using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(ResourceCountIcon))]
public class ResourceCountIconEditor : Editor
{
    private ResourceCountIcon _instance;

    void OnEnable()
    {
        _instance = target as ResourceCountIcon;
    }

    public override void OnInspectorGUI()
    {
        if (_instance == null)
            return;

        DrawDefaultInspector();

        EditorPropertyDrawer.DrawComponentPropertyOfChildObject<Image>(_instance, "m_Sprite");
        EditorPropertyDrawer.DrawComponentPropertyOfChildObject<TextMeshProUGUI>(_instance, "m_text");
    }
}
