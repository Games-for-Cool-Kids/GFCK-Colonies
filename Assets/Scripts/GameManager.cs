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
        Debug.Assert(mainCamera != null);

        // Store player camera.
        var playerCamera = mainCamera.GetComponent<PlayerCamera>();
        Debug.Assert(playerCamera != null);

        PlayerCamera = playerCamera;

        // Store hand controller.
        var handController = mainCamera.GetComponent<HandController>();
        Debug.Assert(handController != null);

        HandController = handController;

        // Store world.
        var worldObject = GameObject.Find(GlobalDefines.worldName);
        Debug.Assert(worldObject != null);
        World = worldObject.GetComponent<World>();
        Debug.Assert(World != null);
    }
}
