using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        // Enforce single instance
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Another instance of {typeof(T)} already exists! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}