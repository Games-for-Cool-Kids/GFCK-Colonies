using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerCamera PlayerCamera { get; private set; }
    public HandController HandController { get; private set; }

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Assert(mainCamera != null);

        // Store player camera.
        var playerCamera = mainCamera.GetComponent<PlayerCamera>();
        Debug.Assert(playerCamera != null);

        PlayerCamera = playerCamera;

        // Store hand controller.
        var handController = mainCamera.GetComponent<HandController>();
        Debug.Assert(handController != null);

        HandController = handController;
    }

    void Update()
    {
        
    }
}
