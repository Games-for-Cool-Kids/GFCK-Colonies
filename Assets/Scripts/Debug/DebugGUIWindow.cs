using System;
using UnityEngine;

public class DebugGUIWindow
{
    private Rect _rect = new(20, 20, 160, 100);
    private bool _resizing = false;

    private string _title = "Debug";

    public GUISkin _debugSkin;

    public delegate void ContentDrawer(int windowId);
    private ContentDrawer _contentDrawer;

    public DebugGUIWindow(string title, ContentDrawer contentDrawer, GUISkin debugSkin)
    {
        _title = title;
        _contentDrawer = contentDrawer;
        _debugSkin = debugSkin;
    }

    public void Draw()
    {
        var old_skin = GUI.skin;
        GUI.skin = _debugSkin; // Apply skin

        _rect = GUILayout.Window(0, _rect,
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

        // - Draw Content - 
        _contentDrawer(windowId);
        // ----------------
        
        ResizeButton();

        if (_resizing)
            ResizeWindowToMouse();
        else
            GUI.DragWindow();

        GUI.skin = old_skin; // Reset skin
    }

    private void ResizeWindowToMouse()
    {
        _rect.max = _rect.min + Event.current.mousePosition;
        if (Input.GetMouseButtonUp(0))
            _resizing = false;
    }

    private void ResizeButton()
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
            _resizing = true; // True until left mouse released.
        // --------
    }
}
