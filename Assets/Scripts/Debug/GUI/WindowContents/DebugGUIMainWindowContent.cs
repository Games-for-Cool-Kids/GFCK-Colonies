using System.Collections.Generic;
using UnityEngine;

namespace DebugGUI
{
    internal class DebugGUIMainWindowContent : DebugGUIWindowContent
    {
        private List<DebugGUIWindow> _windows;

        public DebugGUIMainWindowContent(List<DebugGUIWindow> windows)
        {
            _windows = windows;
        }

        public override void DrawGUI(int windowId)
        {
            GUILayout.BeginVertical();

            foreach (var window in _windows)
                if (GUILayout.Button(window.Title))
                    window.Open = true;

            GUILayout.EndVertical();
        }
    }
}
