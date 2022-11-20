using System.Collections.Generic;
using UnityEngine;

namespace DebugGUI
{
    public class DebugGUI : MonoBehaviour
    {
        public GUISkin DebugSkin;

        public Texture2D DeliveryArrowTex;
        public Texture2D PickupArrowTex;


        [SerializeField, HideInInspector]
        private List<DebugGUIWindow> _windows = new ();

        //internal class TestContent : DebugGUIWindowContent
        //{
        //    public override void DrawGUI(int windowId)
        //    {
        //        GUILayout.BeginVertical();
        //        GUILayout.Label("Test");
        //        GUILayout.EndVertical();
        //    }
        //}

        private void OnEnable()
        {
            _windows.Add(new DebugGUIWindow("Requests", new DebugGUIRequestsWindowContent(DeliveryArrowTex, PickupArrowTex), DebugSkin));
            //_windows.Add(new DebugGUIWindow("Test", new TestContent(), DebugSkin));
        }

        private void OnGUI()
        {
            foreach (var window in _windows)
                if(window.Open)
                    window.Draw();
        }
    }
}
