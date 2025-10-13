using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InformationUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject uiObj;
    private Outline outline;
    private bool isUi = false;
    private bool isSelect = false;
    private Vector3 originalScales;

    private void Awake()
    {
        originalScales = transform.localScale;
        outline = GetComponent<Outline>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
        isSelect = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
        isSelect = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.DOScale(originalScales * 0.9f, 0.1f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isSelect)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.DOScale(originalScales, 0.1f).SetEase(Ease.OutQuad);
                isUi = !isUi;

                if (isUi)
                {
                    uiObj.SetActive(true);
                    uiObj.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
                }
                else
                {
                    uiObj.transform.localScale = Vector3.zero;
                    uiObj.SetActive(false);
                }
            }
        }
    }

    public void Xbutton()
    {
        isUi = false;
        uiObj.SetActive(false);
    }
}
