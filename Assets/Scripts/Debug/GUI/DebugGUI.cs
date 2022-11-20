using UnityEngine;

namespace DebugGUI
{
    public class DebugGUI : MonoBehaviour
    {
        public GUISkin DebugSkin;

        public Texture2D DeliveryArrowTex;
        public Texture2D PickupArrowTex;


        [SerializeField, HideInInspector]
        private DebugGUIWindow _requestsWindow;

        private void OnEnable()
        {
            _requestsWindow = new DebugGUIWindow("Requests", new DebugGUIRequestsWindowContent(DeliveryArrowTex, PickupArrowTex), DebugSkin);
        }

        private void OnGUI()
        {
            _requestsWindow.Draw();
        }
    }
}
