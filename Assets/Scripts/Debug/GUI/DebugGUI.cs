using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace DebugGUI
{
    public partial class DebugGUI : MonoBehaviour
    {
        public bool Show = true;

        public GUISkin DebugSkin;

        public Texture2D DeliveryArrowTex;
        public Texture2D PickupArrowTex;


        [SerializeField, HideInInspector]
        private List<DebugGUIWindow> _windows = new();
        private DebugGUIWindow _mainDebugWindow;


        private void OnEnable()
        {
            int id = 0;

            _mainDebugWindow = new DebugGUIWindow(++id, "DebugWindows - F5", new DebugGUIMainWindowContent(_windows));
            _mainDebugWindow.Open = true;

            _windows.Add(new DebugGUIWindow(++id, "Requests", new DebugGUIRequestsWindowContent(DeliveryArrowTex, PickupArrowTex)));
            _windows.Add(new DebugGUIWindow(++id, "Units", new DebugGUIUnitWindowContent()));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Show = !Show;
                _mainDebugWindow.Open = Show;
            }
        }

        void OnGUI()
        {
            if (!Show)
                return;

            GUI.skin = DebugSkin; // Apply skin

            _mainDebugWindow.Draw();

            foreach (var window in _windows)
                if (window.Open)
                    window.Draw();

            GUI.skin = null;
        }
    }
}
