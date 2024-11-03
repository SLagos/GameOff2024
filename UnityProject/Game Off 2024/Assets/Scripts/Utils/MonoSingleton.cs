using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T _instance;
    public static T Instance;

    [SerializeField] private bool _dontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            Instance = _instance;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    protected virtual void Start(){
        
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void OnApplicationQuit()
    {
        _instance = null;
    }

}