using UnityEngine;
using UnityEditor;

// Add one for each class derived from building.
[CustomEditor(typeof(LumberMill))] public class LumberMillEditor : BuildingEditor
{ public override void OnInspectorGUI() { base.OnInspectorGUI(); } }
[CustomEditor(typeof(Stockpile))] public class StockpileEditor : BuildingEditor
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


        _showGrid = EditorGUILayout.Foldout(_showGrid, "Grid");
        if (_showGrid)
        {
            var grid = _building.grid;

            EditorGUI.indentLevel = 0;

            grid.width = EditorGUILayout.IntField("Width", _building.grid.width);
            grid.length = EditorGUILayout.IntField("Length", _building.grid.length);

            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(10, 10, 10, 10);
            tableStyle.margin.left = 32;

            GUIStyle columnStyle = new GUIStyle();
            columnStyle.fixedWidth = 65;

            EditorGUILayout.BeginVertical(columnStyle);
            for (int y = 0; y < grid.length; y++)
            {
                EditorGUILayout.BeginHorizontal(columnStyle);
                for (int x = 0; x < grid.width; x++)
                {
                    grid.grid[x, y] = (BuildingGrid.Cell)EditorGUILayout.EnumPopup(grid.grid[x, y]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            //serializedObject.Update();
        }
    }
}
