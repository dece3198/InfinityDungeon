using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField] private int levelIndex;
    public int LevelIndex
    {
        get { return levelIndex; }
        set 
        { 
            levelIndex = value; 
            levelText.text = levelIndex.ToString() + " / " + level.ToString();
        }
    }
    public int level;
    [SerializeField] private List<GameObject> curUnits = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private UnitSlot[] waitSlot;

    private void Start()
    {
        level = 3;
        LevelIndex = 0;
        /*
        for(int i = 0; i < GameManager.instance.mercenaryList.Count; i++)
        {
            GameObject unit = Instantiate(GameManager.instance.mercenaryList[i].unitPrefab);
            curUnits.Add(unit);
            AddUnit(unit.GetComponent<UnitController>());
        }
        */
    }
    
    public void GameStart()
    {
        for(int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i].TryGetComponent(out Rigidbody r))
            {
                r.isKinematic = false;
            }
        }
    }

    private void AddUnit(UnitController unit)
    {
        for(int i = 0; i < waitSlot.Length; i++)
        {
            if(waitSlot[i].controller == null)
            {
                waitSlot[i].AddUnit(unit);
                return;
            }
        }
    }
}
