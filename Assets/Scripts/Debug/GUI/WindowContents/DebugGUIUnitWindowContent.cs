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

        private static void DrawUnitGUI(Unit unit)
        {
            GUILayout.BeginHorizontal();
            // ------------------------

            GUILayout.Label(unit.gameObject.name);

            var job = unit.GetComponent<UnitComponentJob>().job;
            if (job != null)
            {
                if (GUILayout.Button(job.type.ToString()))
                    DebugGUI.Instance.Select(job);
            }
            else
            {
                GUILayout.Label("Unemployed");
            }

            // ------------------------
            GUILayout.EndHorizontal();
        }
    }
}
