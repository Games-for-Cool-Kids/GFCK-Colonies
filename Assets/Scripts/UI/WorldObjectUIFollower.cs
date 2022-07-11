using UnityEngine;

public class WorldObjectUIFollower : MonoBehaviour
{
    public GameObject ObjectToFollow;

    private RectTransform _parentCanvas;

    void Start()
    {
        if(ObjectToFollow == null)
        {
            ObjectToFollow = transform.root.gameObject; // By default follow top-most parent.
        }

        _parentCanvas = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        var canvas = GetComponent<RectTransform>();

        Vector3 canvasPos = Camera.main.WorldToScreenPoint(ObjectToFollow.transform.position);
        canvasPos.x -= _parentCanvas.sizeDelta.x / 2;
        canvasPos.y -= _parentCanvas.sizeDelta.y / 2;

        canvas.localPosition = canvasPos;
    }
}
