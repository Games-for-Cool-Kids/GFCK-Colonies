using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Economy;

namespace DebugGUI
{
    internal class DebugGUIRequestsWindowContent : DebugGUIWindowContent
    {
        private Texture2D _deliveryArrowTex;
        private Texture2D _pickupArrowTex;

        private bool _showOpenRequests = false;
        private bool _showPromisedRequests = false;
        private bool _showPickupRequests = false;
        private bool _showDeliveryRequests = false;

        public DebugGUIRequestsWindowContent(Texture2D deliveryArrowTex, Texture2D pickupArrowTex)
        {
            _deliveryArrowTex = deliveryArrowTex;
            _pickupArrowTex = pickupArrowTex;
        }

        public override void DrawGUI(int windowId)
        {
            var request_manager = PlayerInfo.Instance.ResourceTransferRequestManager;

            var open_requests = request_manager.GetOpenRequests();
            var promised_requests = request_manager.GetPromisedRequests();
            var pickup_requests = request_manager.GetPickupRequests()
                .Select(req => req as ResourceTransferRequest).ToList();
            var delivery_requests = request_manager.GetDeliveryRequests()
                .Select(req => req as ResourceTransferRequest).ToList();

            // - Draw 
            GUILayout.BeginVertical();

            DrawRequestList(open_requests, "Open", ref _showOpenRequests);
            DrawRequestList(pickup_requests, "Pickup", ref _showPickupRequests);
            DrawRequestList(delivery_requests, "Delivery", ref _showDeliveryRequests);
            DrawPromisedRequestList(promised_requests, "Promised", ref _showPromisedRequests);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            // --------
        }

        private void DrawRequestList(IEnumerable<ResourceTransferRequest> requests, string title, ref bool foldout)
        {
            if (foldout = EditorGUILayout.Foldout(foldout, title + " - " + requests.Count()))
                foreach (var request in requests)
                    DrawRequest(request);
        }

        private void DrawRequest(ResourceTransferRequest request)
        {
            string label = string.Format("{0}x{1} {2} {3}",
                request.Amount,
                request.ResourceType.ToString(),
                request is ResourcePickUpRequest ? "from" : "to",
                request.Requester.gameObject.name);

            // - Draw -
            GUILayout.BeginHorizontal();

            DrawRequestArrow(request);
            GUILayout.Label(label);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            // --------
        }

        private void DrawPromisedRequestList(IEnumerable<(Unit unit, ResourceTransferRequest request)> requests, string title, ref bool foldout)
        {
            if (foldout = EditorGUILayout.Foldout(foldout, title + " - " + requests.Count()))
                foreach (var request in requests)
                    DrawPromisedRequest(request);
        }

        private void DrawPromisedRequest((Unit unit, ResourceTransferRequest request) promise)
        {
            string label = string.Format("{0} will {1} {2}x{3} {4} {5}",
                promise.unit.gameObject.name,
                promise.request is ResourcePickUpRequest ? "pick up" : "deliver",
                promise.request.Amount,
                promise.request.ResourceType.ToString(),
                promise.request is ResourcePickUpRequest ? "from" : "to",
                promise.request.Requester.gameObject.name);

            GUILayout.BeginHorizontal();

            DrawRequestArrow(promise.request);
            GUILayout.Label(label);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawRequestArrow(ResourceTransferRequest req)
        {
            const int arrow_size = 8;
            var arrow_tex = req is ResourcePickUpRequest ? _pickupArrowTex : _deliveryArrowTex;

            GUILayout.BeginVertical();

            GUILayout.Space(4);
            GUILayout.Box(arrow_tex, GUILayout.Width(arrow_size), GUILayout.Height(arrow_size));

            GUILayout.EndVertical();
        }
    }
}
