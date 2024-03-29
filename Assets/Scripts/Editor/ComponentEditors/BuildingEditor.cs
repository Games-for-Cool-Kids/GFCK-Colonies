﻿using UnityEngine;
using UnityEditor;

// Add one for each class derived from building.
[CustomEditor(typeof(LumberMill))]
public class LumberMillEditor : BuildingEditor
{ public override void OnInspectorGUI() { base.OnInspectorGUI(); } }
[CustomEditor(typeof(Stockpile))]
public class StockpileEditor : BuildingEditor
{ public override void OnInspectorGUI() { base.OnInspectorGUI(); } }
[CustomEditor(typeof(TownCenter))]
public class TownCenterEditor : BuildingEditor
{ public override void OnInspectorGUI() { base.OnInspectorGUI(); } }


[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    private Building _building;

    private bool _showGrid = true;

    void OnEnable()
    {
        _building = serializedObject.targetObject as Building;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var inventory = _building.inventory;
        InventoryEditor.DrawInventory(inventory);

        _showGrid = EditorGUILayout.Foldout(_showGrid, "Grid");
        if (_showGrid)
        {
            var buildGrid = _building.buildGrid;

            EditorGUI.indentLevel = 0;

            buildGrid.width = EditorGUILayout.IntField("Width", _building.buildGrid.width);
            buildGrid.length = EditorGUILayout.IntField("Length", _building.buildGrid.length);

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 65;

            EditorGUILayout.BeginVertical(columnStyle);
            for (int y = 0; y < buildGrid.length; y++)
            {
                EditorGUILayout.BeginHorizontal(columnStyle);
                for (int x = 0; x < buildGrid.width; x++)
                {
                    buildGrid.grid[x, y] = (BuildingGrid.Cell)EditorGUILayout.EnumPopup(buildGrid.grid[x, y]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            serializedObject.Update();
        }
    }
}
