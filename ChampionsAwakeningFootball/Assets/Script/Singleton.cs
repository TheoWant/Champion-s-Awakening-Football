using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// It's a singleton that inherits MonoBehaviour
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SingletonAwake();
    }

    public virtual void SingletonAwake()
    {
    }
}

/// <summary>
/// It's a singleton that inherits MonoBehaviour and doesn't destroy when changing scene
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}