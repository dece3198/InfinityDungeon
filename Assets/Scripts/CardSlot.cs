using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Card card;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image typeImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelText2;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Transform legendCard;
    private Vector3 orignScale;
    private bool isSelect = false;
    public bool isFlip = false;

    private void Awake()
    {
        orignScale = transform.localScale;
    }

    public void CardFlip()
    {
        StartCoroutine(CardFlipCo());
    }

    public IEnumerator CardFlipCo()
    {
        isSelect = false;
        transform.DOScale(transform.localScale * 1.2f, 0.25f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOLocalRotate(new Vector3(0, isFlip ? 0f : 180f, 0), 0.3f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        isFlip = true;
                        transform.DOScale(orignScale, 0.25f)
                            .SetEase(Ease.OutBack);
                        if(card.cardRank == CardRank.Legend)
                        {
                            legendCard.gameObject.SetActive(true);
                            legendCard.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
                            
                        }
                    });
            });
        yield return new WaitForSeconds(0.8f);
        isSelect = true;
    }

    public void AddCard(Card _card, Sprite rank)
    {
        card = _card;
        rankImage.sprite = rank;
        typeImage.sprite = card.typeImage;
        unitImage.sprite = card.unitImage;
        if (card.cardRank > CardRank.Gold)
        {
            levelText2.text = card.level.ToString();
            levelText2.gameObject.SetActive(true);
            levelText.gameObject.SetActive(false);
        }
        else
        {
            levelText.text = card.level.ToString();
            levelText2.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
        }
        
        valueText.text = card.value.ToString();
        nameText.text = card.cardName;
    }

    public void ClearCard()
    {
        card = null;
        isFlip = false;
        legendCard.gameObject.SetActive(false);
        legendCard.localScale = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isSelect)
        {
            transform.DOScale(orignScale * 1.2f, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelect)
        {
            transform.DOScale(orignScale, 0.25f).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isSelect)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.DOScale(orignScale, 0.25f).SetEase(Ease.OutQuad);
                CardManager.instance.SelectCard(this);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
