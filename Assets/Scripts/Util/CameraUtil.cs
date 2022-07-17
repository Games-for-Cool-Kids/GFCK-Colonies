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

    static public Ray GetMouseRay()
    {
        Vector3 near = GetWorldPosMouseNear();
        Vector3 far = GetWorldPosMouseFar();
        return new Ray(near, far - near);
    }

    public static RaycastHit CastMouseRayFromCamera(int layerMask = 0)
    {
        Vector3 worldMousePosNear = CameraUtil.GetWorldPosMouseNear();
        Vector3 worldMousePosFar = CameraUtil.GetWorldPosMouseFar();

        RaycastHit rayHit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out rayHit, layerMask);

        return rayHit;
    }
}
