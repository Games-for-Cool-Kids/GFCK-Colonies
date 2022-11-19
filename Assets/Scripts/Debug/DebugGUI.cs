using UnityEditor;
using UnityEngine;
using Economy;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;

public class DebugGUI : MonoBehaviour
{
    public GUISkin DebugSkin;

    public Texture2D DeliveryArrowTex;
    public Texture2D PickupArrowTex;

    private bool _showOpenRequests = false;
    private bool _showPromisedRequests = false;
    private bool _showPickupRequests = false;
    private bool _showDeliveryRequests = false;

    [SerializeField, HideInInspector]
    private DebugGUIWindow _requestsWindow;

    private void OnEnable()
    {
        _requestsWindow = new DebugGUIWindow("Requests", DrawRequestsWindow, DebugSkin);

    }

    private void OnGUI()
    {
        _requestsWindow.Draw();
    }

    private void DrawRequestsWindow(int windowId)
    {
        var request_manager = PlayerInfo.Instance.ResourceTransferRequestManager;

        var open_requests = request_manager.GetOpenRequestsClone();
        var promised_requests = request_manager.GetPromisedRequestsClone();
        var pickup_requests = request_manager.GetPickupRequestsClone()
            .Select(req => req as ResourceTransferRequest).ToList();
        var delivery_requests = request_manager.GetDeliveryRequestsClone()
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

    private void DrawRequestList(List<ResourceTransferRequest> requests, string title, ref bool foldout)
    {
        if (foldout = EditorGUILayout.Foldout(foldout, title + " - " + requests.Count))
            foreach (var request in requests)
                DrawRequest(request);
    }

    private void DrawRequest(ResourceTransferRequest request)
    {
        string label = string.Format("{0}x{1} {2} {3}",
            request.resourceStack.amount,
            request.resourceStack.type.ToString(),
            request is ResourcePickUpRequest ? "from" : "to",
            request.requester.gameObject.name);

        // - Draw -
        GUILayout.BeginHorizontal();

        DrawRequestArrow(request);
        GUILayout.Label(label);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        // --------
    }

    private void DrawPromisedRequestList(List<(Unit unit, ResourceTransferRequest request)> requests, string title, ref bool foldout)
    {
        if (foldout = EditorGUILayout.Foldout(foldout, title + " - " + requests.Count))
            foreach (var request in requests)
                DrawPromisedRequest(request);
    }

    private void DrawPromisedRequest((Unit unit, ResourceTransferRequest request) promise)
    {
        string label = string.Format("{0} will {1} {2}x{3} {4} {5}",
            promise.unit.gameObject.name,
            promise.request is ResourcePickUpRequest ? "pick up" : "deliver",
            promise.request.resourceStack.amount,
            promise.request.resourceStack.type.ToString(),
            promise.request is ResourcePickUpRequest ? "from" : "to",
            promise.request.requester.gameObject.name);

        GUILayout.BeginHorizontal();

        DrawRequestArrow(promise.request);
        GUILayout.Label(label);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawRequestArrow(ResourceTransferRequest req)
    {
        const int arrow_size = 8;
        var arrow_tex = req is ResourcePickUpRequest ? PickupArrowTex : DeliveryArrowTex;

        GUILayout.BeginVertical();

        GUILayout.Space(4);
        GUILayout.Box(arrow_tex, GUILayout.Width(arrow_size), GUILayout.Height(arrow_size));

        GUILayout.EndVertical();
    }
}
