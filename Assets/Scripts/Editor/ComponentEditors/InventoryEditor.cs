using UnityEngine;
using UnityEditor;
using Economy;
using System;

// Add one for each class derived from StorageEntity.
// For buildings this is done by BuildingEditor.
[CustomEditor(typeof(Unit))]
public class UnitInventoryEditor : InventoryEditor
{ public override void OnInspectorGUI() { base.OnInspectorGUI(); } }


[CustomEditor(typeof(StorageEntity))]
public class InventoryEditor : Editor
{
    private StorageEntity _instance;

    void OnEnable()
    {
        _instance = serializedObject.targetObject as StorageEntity;
    }

    public override void OnInspectorGUI()
    {
        if (_instance == null)
            return;

        DrawDefaultInspector();

        var inventory = _instance.inventory;
        DrawInventory(inventory);

        serializedObject.Update();
    }

    public static void DrawInventory(Inventory inventory)
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int value = EditorGUILayout.IntField(
                type.ToString(),
                inventory.GetCount(type));

            if (value <= 0)
                value = 0;

            inventory.SetResource(type, value);
        }
    }
}
