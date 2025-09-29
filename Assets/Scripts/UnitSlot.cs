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

        if (MouseController.instance.curSlot == this) return;

        if(!check.activeInHierarchy)
            check.SetActive(true);

        if (MouseController.instance.curSlot != null && MouseController.instance.curSlot != this)
            MouseController.instance.curSlot.ClearSlot();

        MouseController.instance.curSlot = this;
        controller = unit;
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

    public void ClearSlot()
    {
        check.SetActive(false);
        controller = null;
    }
}
