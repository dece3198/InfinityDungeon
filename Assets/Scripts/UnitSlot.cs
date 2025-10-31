using UnityEngine;

public class UnitSlot : MonoBehaviour
{
    public UnitController controller;
    public GameObject check;
    public UnitSlotType slotType;

    private void OnTriggerStay(Collider other)
    {
        var unit = other.GetComponent<UnitController>();
        if (unit == null) return;

        if (!unit.isSelect) return;

        if (!check.activeInHierarchy)
            check.SetActive(true);
        unit.unitSlot = this;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<UnitController>() != null)
        {
            if (check.activeInHierarchy)
            {
                check.SetActive(false);
            }
        }
    }

    public void AddUnit(UnitController unit)
    {
        unit.transform.position = transform.position;
        unit.gameObject.SetActive(true);
        controller = unit;
    }

    public void ClearSlot()
    {
        check.SetActive(false);
        controller = null;
    }
}
