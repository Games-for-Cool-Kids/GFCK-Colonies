using Jobs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

namespace DebugGUI
{
    public partial class DebugGUI : MonoBehaviourSingleton<DebugGUI>
    {
        public bool Show = true;

        public GUISkin DebugSkin;

        public Texture2D DeliveryArrowTex;
        public Texture2D PickupArrowTex;


        [SerializeField, HideInInspector]
        private List<DebugGUIWindow> _windows = new();
        [SerializeField] private DebugGUIWindow _mainDebugWindow;


        protected override void OnEnable()
        {
            base.OnEnable();

            _windows.Clear();

            int id = 0;

            _mainDebugWindow = new DebugGUIWindow(++id, "DebugWindows - F5", new DebugGUIMainWindowContent(_windows));
            _mainDebugWindow.Open = true;

            var requests_window = new DebugGUIWindow(++id, "Requests", new DebugGUIRequestsWindowContent(DeliveryArrowTex, PickupArrowTex));
            requests_window.Rect.y = _mainDebugWindow.Rect.yMax + 10;
            var units_window = new DebugGUIWindow(++id, "Units", new DebugGUIUnitWindowContent());
            units_window.Rect.y = _mainDebugWindow.Rect.yMax + 10;
            var jobs_window = new DebugGUIWindow(++id, "Jobs", new DebugGUIJobsWindowContent());
            jobs_window.Rect.y = _mainDebugWindow.Rect.yMax + 10;


            _windows.Add(requests_window);
            _windows.Add(units_window);
            _windows.Add(jobs_window);
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

        public void Select(Job job)
        {
            var job_window = _windows.Find(wnd => wnd.Title == "Jobs");

            job_window.Open = true;
            job_window.Minimized = false;
        }
    }
}
