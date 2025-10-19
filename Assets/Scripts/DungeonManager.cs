using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private UnitSlot[] waitSlot;

    private void Start()
    {
        for(int i = 0; i < GameManager.instance.mercenaryList.Count; i++)
        {
            GameObject unit = Instantiate(GameManager.instance.mercenaryList[i].unitPrefab);
            AddUnit(unit.GetComponent<UnitController>());
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
