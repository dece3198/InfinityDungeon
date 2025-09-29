using UnityEngine;

public class UnitController : MonoBehaviour
{

    public Vector3 orignPos;

    public void OnSelect()
    {
        //잡을때 효과
        orignPos = transform.position;
    }

    public void FollowMouse(Vector3 targetPos)
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }

    public void OnRelease()
    {
        if(MouseController.instance.curSlot != null)
        {
            transform.position = MouseController.instance.curSlot.transform.position;
        }
        else
        {
            transform.position = orignPos;
        }
    }
}
