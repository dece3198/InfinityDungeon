using UnityEngine;
using UnityEngine.UI;

public class InsigniaSlot : MonoBehaviour
{
    [SerializeField] private Image insigniaImage;
    public Insignia insignia;

    public void AddSlot(Insignia _insignia)
    {
        insignia = _insignia;
        if(insignia != null)
        {
            insigniaImage.sprite = insignia.insigniaImage;
        }
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        insignia = null;
        gameObject.SetActive(false);
    }
}
