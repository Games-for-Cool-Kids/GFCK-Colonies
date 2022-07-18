using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public PlayerCamera PlayerCamera { get; private set; }
    public HandController HandController { get; private set; }

    public World World { get; private set; }


    private new void Awake()
    {
        base.Awake();


        GameObject mainCamera = GameObject.FindGameObjectWithTag(GlobalDefines.mainCameraTag);
        if(mainCamera == null)
            Debug.LogWarning(GlobalDefines.mainCameraTag + " not found.");

        // Store player camera.
        var playerCamera = mainCamera.GetComponent<PlayerCamera>();
        PlayerCamera = playerCamera;
        if (playerCamera == null)
            Debug.LogWarning("PlayerCamera not found.");


        // Store hand controller.
        var handController = mainCamera.GetComponent<HandController>();
        HandController = handController;
        if (handController == null)
            Debug.LogWarning("HandController not found.");


        // Store world.
        var worldObject = GameObject.Find(GlobalDefines.worldName);
        World = worldObject.GetComponent<World>();
        if (worldObject == null || World == null)
            Debug.LogWarning("World not found.");
    }
}
