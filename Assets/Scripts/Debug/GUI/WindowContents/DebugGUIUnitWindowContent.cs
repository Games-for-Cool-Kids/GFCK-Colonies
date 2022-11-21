
using System.Linq;
using UnityEngine;

namespace DebugGUI
{
    internal class DebugGUIUnitWindowContent : DebugGUIWindowContent
    {
        public override void DrawGUI(int windowId)
        {
            var units = PlayerInfo.Instance.playerFaction.Population
                .ConvertAll(unit => unit.GetComponent<Unit>())
                .ToList();

            foreach(Unit unit in units)
            {
                DrawUnitGUI(unit);
            }
        }

        private void DrawUnitGUI(Unit unit)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(unit.gameObject.name);

            GUILayout.EndHorizontal();
        }
    }
}
