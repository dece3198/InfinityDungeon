using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T _instance;
    public static T instance
    {
        get
        {
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_instance != null)
        {
            Destroy(this.gameObject);
        }

    }
}
