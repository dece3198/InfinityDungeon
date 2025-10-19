using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<Mercenary> mercenaryList = new List<Mercenary>();
    public bool isStart = true;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
