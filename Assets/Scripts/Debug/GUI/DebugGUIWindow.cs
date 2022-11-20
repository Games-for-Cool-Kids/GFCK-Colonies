using System;
using UnityEngine;

namespace DebugGUI
{
    internal class DebugGUIWindow
    {
        public bool Open = true;
        public bool Minimized = false;

        private Rect _rect = new(20, 20, 160, 100);
        private bool _resizing = false;

        private string _title = "Debug";

        private GUISkin _debugSkin;

        private DebugGUIWindowContent _content;

        public DebugGUIWindow(string title, DebugGUIWindowContent content, GUISkin debugSkin)
        {
            _title = title;
            _content = content;
            _debugSkin = debugSkin;
        }

        public void Draw()
        {
            var old_skin = GUI.skin;
            GUI.skin = _debugSkin; // Apply skin

            _rect = GUILayout.Window(0,
                                     _rect,
                                     DrawInternal,
                                     _title,
                                     GUILayout.MaxHeight(Screen.height - _rect.y),
                                     GUILayout.MaxWidth(Screen.width - _rect.x));

            GUI.skin = old_skin; // Reset skin
        }

        private void DrawInternal(int windowId)
        {
            var old_skin = GUI.skin;
            GUI.skin = _debugSkin; // Apply skin

            if (!Minimized)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(22);
                // - Draw Content - 
                _content.DrawGUI(windowId);
                // ----------------
                GUILayout.EndVertical();

                DrawResizeButton();
            }
            else
            {
                _rect.height = 23;
                DrawMinimized();
            }

            DrawMinMaxButton();

            if (_resizing)
                ResizeWindowToMouse();
            else
                GUI.DragWindow();

            GUI.skin = old_skin; // Reset skin
        }

        private void DrawMinimized()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(1);
            GUILayout.EndVertical();
        }

        private void ResizeWindowToMouse()
        {
            _rect.max = _rect.min + Event.current.mousePosition;
            if (Input.GetMouseButtonUp(0))
                _resizing = false;
        }

        private void DrawResizeButton()
        {
            const int btn_size = 8;
            var btn_rect = new Rect(_rect.width - btn_size - 1,
                                    _rect.height - btn_size - 1,
                                    btn_size,
                                    btn_size);

            // - Draw -
            GUIUtil.ColoredRect(btn_rect, new(1, 1, 1, 0.2f));

            if (GUI.RepeatButton(btn_rect, "", GUIStyle.none)
             && Input.GetMouseButton(0))
            {
                _resizing = true; // True until left mouse released.
            }
            // --------
        }


        private void DrawMinMaxButton()
        {
            const int btn_size = 10;
            var btn_rect = new Rect(_rect.width - btn_size - 2,
                                    2,
                                    btn_size,
                                    btn_size);

            // - Draw -
            GUIUtil.ColoredRect(btn_rect, new(1, 1, 1, 0.2f));

            if (GUI.Button(btn_rect, Minimized ? "+" : "-"))
                Minimized = !Minimized;
            // --------
        }

    }
}
