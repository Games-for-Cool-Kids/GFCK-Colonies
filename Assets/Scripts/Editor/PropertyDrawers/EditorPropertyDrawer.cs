using UnityEngine;
using UnityEditor;

public class EditorPropertyDrawer
{
    static public void DrawComponentPropertyOfChildObject<ComponentType>(Component component, string propertyName) where ComponentType : Component
    {
        ComponentType childComponent = component.GetComponentInChildren<ComponentType>();
        if (component == null)
            return;

        DrawProperty(childComponent, propertyName);
    }

    static public void DrawProperty(Component component, string propertyName)
    {
        SerializedObject componentSO = new SerializedObject(component);
        if (componentSO == null)
            return;

        SerializedProperty property = componentSO.GetIterator();
        while (property.NextVisible(true))
        {
            if (property.name == propertyName)
            {
                EditorGUILayout.PropertyField(property);
            }
        }
        property.Reset();

        componentSO.ApplyModifiedProperties();
    }
}
