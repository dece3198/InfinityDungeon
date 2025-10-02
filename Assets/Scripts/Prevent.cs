using UnityEngine;

public class Prevent : MonoBehaviour
{
    private bool trigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (trigger) return;

        if(other.GetComponent<UnitController>() != null)
        {
            trigger = true;
            MouseController.instance.MouseUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<UnitController>() != null)
        {
            trigger = false;
        }
    }
}
