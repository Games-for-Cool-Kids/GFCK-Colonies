using UnityEngine;

public class CameraUtil
{
    static public Vector3 GetWorldPosMouseNear()
    {
        Vector3 screenMousePos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
    static public Vector3 GetWorldPosMouseFar()
    {
        Vector3 screenMousePos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);

        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }

    static public Ray GetRayFromCameraToMouse()
    {
        Vector3 near = GetWorldPosMouseNear();
        Vector3 far = GetWorldPosMouseFar();
        return new Ray(near, far - near);
    }
}
