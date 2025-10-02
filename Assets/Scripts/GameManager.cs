using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
