using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Debug.LogWarning("Only one instance of singleton allowed. Duplicate exists on object: " + this.gameObject.name);
            Destroy(gameObject); // Only one object of type allowed.
        }
    }

    protected virtual void OnEnable()
    {
        if (Instance == null)
        {
            CreateInstance();
        }
    }

    protected virtual void Awake()
    {
        CreateInstance();
    }
}