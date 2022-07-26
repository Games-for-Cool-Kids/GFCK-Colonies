using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
	public static T Instance { get; private set; }

	public virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
		}
		else
		{
			Debug.LogError("Only one instance of singleton allowed. Duplicate exists on object: " + this.name);
			Destroy(gameObject); // Only one object of type allowed.
		}
	}
}