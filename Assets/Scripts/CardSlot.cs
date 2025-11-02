using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Card card;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private Image typeImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelText2;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;
    private Vector3 orignScale;
    private bool isSelect = true;
    private bool isFlip = false;

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
                transform.DORotate(new Vector3(0, isFlip ? 0f : 180f, 0), 0.3f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        transform.DOScale(orignScale, 0.25f)
                            .SetEase(Ease.OutBack);
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
}
