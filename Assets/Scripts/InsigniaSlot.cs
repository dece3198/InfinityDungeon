using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsigniaSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private Image insigniaImage;
    [SerializeField] private TextMeshProUGUI rankText;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        rankText.gameObject.transform.position = eventData.position;
        rankText.gameObject.SetActive(true);
        rankText.text = insignia.insigniaRank.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rankText.gameObject.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        rankText.gameObject.transform.position = eventData.position;
    }
}
