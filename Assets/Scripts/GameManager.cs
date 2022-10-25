using UnityEngine;
using World;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public PlayerCamera PlayerCamera { get; private set; }
    public HandController HandController { get; private set; }

    public GameWorld World { get; private set; }

    public delegate void GameObjectEvent(GameObject gameObject);
    public event GameObjectEvent gameObjectCreate;

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
        World = worldObject.GetComponent<GameWorld>();
        if (worldObject == null || World == null)
            Debug.LogWarning("GameWorld not found.");
    }

    public GameObject InstantiateGameObject(GameObject obj)
    {
        var newObject = GameObject.Instantiate(obj);
        gameObjectCreate?.Invoke(newObject); // Send out event.
        return newObject;
    }
}
