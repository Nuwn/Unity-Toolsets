using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static T Instance { get; protected set; }

    public Singleton()
    {
        Instance = (T)(object)this;
    }

    public virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
