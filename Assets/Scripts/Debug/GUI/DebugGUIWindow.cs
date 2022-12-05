using System;
using UnityEngine;

namespace DebugGUI
{
    [Serializable]
    internal class DebugGUIWindow
    {
        public string Title = "Debug";
        public int ID { get; private set; }

        public bool Open = false;
        public bool Minimized = false;

        [SerializeField] public Rect Rect = new(20, 20, 165, 100);
        [SerializeField] private bool _resizing = false;

        [SerializeField] private DebugGUIWindowContent _content;

        public DebugGUIWindow(int id, string title, DebugGUIWindowContent content)
        {
            Title = title;
            ID = id;
            _content = content;
        }

        public void Draw()
        {
            Rect = GUILayout.Window(ID,
                                     Rect,
                                     DrawInternal,
                                     Title,
                                     WindowStyle(),
                                     GUILayout.MaxHeight(Screen.height - Rect.y),
                                     GUILayout.MaxWidth(Screen.width - Rect.x));
        }

        private void DrawInternal(int windowId)
        {
            if (!Minimized)
            {
                GUILayout.BeginVertical();
                // - Draw Content - 
                _content.DrawGUI(windowId);
                // ----------------
                GUILayout.EndVertical();

                DrawResizeButton();
            }
            else
            {
                Rect.height = 23;
                DrawMinimized();
            }

            DrawMinMaxButton();
            DrawCloseButton();

            if (_resizing)
                ResizeWindowToMouse();
            else
                GUI.DragWindow();
        }

        private GUIStyle WindowStyle()
        {
            var style = new GUIStyle(GUI.skin.window);
            if(Minimized)
            {
                style.padding = new(0, 0, 5, 0);
                style.contentOffset = Vector2.zero;
            }
            else 
            {
                style.padding = new(4, 4, 23, 4);
                style.contentOffset = new(0, -18);
            }
            return style;
        }

        private void DrawMinimized()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(0);
            GUILayout.EndVertical();
        }

        private void ResizeWindowToMouse()
        {
            Rect.max = Rect.min + Event.current.mousePosition;
            if (Input.GetMouseButtonUp(0))
                _resizing = false;
        }

        private void DrawResizeButton()
        {
            const int btn_size = 8;
            var btn_rect = new Rect(Rect.width - btn_size - 1,
                                    Rect.height - btn_size - 1,
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
            var btn_rect = new Rect(Rect.width - 24,
                                    2,
                                    btn_size,
                                    btn_size);

            // - Draw -
            GUIUtil.ColoredRect(btn_rect, new(1, 1, 1, 0.2f));

            if (GUI.Button(btn_rect, Minimized ? "+" : "-"))
                Minimized = !Minimized;
            // --------
        }
        private void DrawCloseButton()
        {
            const int btn_size = 10;
            var btn_rect = new Rect(Rect.width - btn_size - 2,
                                    2,
                                    btn_size,
                                    btn_size);

            // - Draw -
            GUIUtil.ColoredRect(btn_rect, new(1, 1, 1, 0.2f));

            if (GUI.Button(btn_rect, "x"))
                Open = false;
            // --------
        }

    }
}
