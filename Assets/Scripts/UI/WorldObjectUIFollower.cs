using UnityEngine;

public class WorldObjectUIFollower : MonoBehaviour
{
    public GameObject ObjectToFollow; // Follows root parent gameobject if no other object is set. 

    private RectTransform _parentCanvas;

    public float HeightOffset = 0;

    void Start()
    {
        if(ObjectToFollow == null)
        {
            ObjectToFollow = transform.root.gameObject;
        }

        _parentCanvas = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        var canvas = GetComponent<RectTransform>();

        Vector3 canvasPos = Camera.main.WorldToScreenPoint(ObjectToFollow.transform.position + new Vector3(0, HeightOffset, 0));
        canvasPos.x -= _parentCanvas.sizeDelta.x / 2;
        canvasPos.y -= _parentCanvas.sizeDelta.y / 2;

        canvas.localPosition = canvasPos;
    }
}
