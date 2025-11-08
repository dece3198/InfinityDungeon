using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UseCardSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UseCard card;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image rankImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelTextA;
    [SerializeField] private TextMeshProUGUI levelTextB;
    [SerializeField] private TextMeshProUGUI valueTextA;
    [SerializeField] private GameObject select;
    [SerializeField] private Transform usePos;
    [SerializeField] private Transform deletePos;
    public Transform legendCard;
    private RectTransform rect;
    private Vector2 originalLocalPos;
    private Vector3 originalScale;
    public bool isUse = false;
    public bool isFlip = false;
    private bool isSelect = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = transform.localScale;
    }

    public void AddCard(UseCard useCard, Sprite rank)
    {
        card = useCard;
        cardImage.sprite = card.cardImage;
        rankImage.sprite = rank;
        nameText.text = card.cardName;
        if(useCard.cardRank > CardRank.Gold)
        {
            levelTextA.gameObject.SetActive(false);
            levelTextB.gameObject.SetActive(true);
            levelTextB.text = useCard.level.ToString();
        }
        else
        {
            levelTextA.gameObject.SetActive(true);
            levelTextB.gameObject.SetActive(false);
            levelTextA.text = useCard.level.ToString();
        }

        if(card.useType == UseType.MonsterStun)
        {
            valueTextA.text = (useCard.level * 0.1f).ToString("N0") + "s";
        }
        else
        {
            valueTextA.text = useCard.level.ToString() + "%";
        }
    }


    public void ClearCard()
    {
        card = null;
        transform.localEulerAngles = Vector3.zero;
        legendCard.localScale = Vector3.zero;
        legendCard.gameObject.SetActive(false);
        gameObject.SetActive(false);
        select.SetActive(false);
        isUse = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isUse) return;

        isSelect = !isSelect;

        if (isSelect)
        {
            select.SetActive(true);

            rect.DOKill();
            transform.DOKill();

            originalLocalPos = rect.localPosition;
            rect.DOAnchorPos(originalLocalPos + new Vector2(0, 150f), 0.18f).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale * 1.2f, 0.18f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                CardSlotManager.instance.useButton.gameObject.SetActive(true);
                CardSlotManager.instance.deleteButton.gameObject.SetActive(true);
                CardSlotManager.instance.useButton.SetParent(usePos, false);
                CardSlotManager.instance.useButton.localPosition = Vector3.zero;
                CardSlotManager.instance.useButton.localEulerAngles = new Vector3(0, 180f, 0);
                CardSlotManager.instance.deleteButton.SetParent(deletePos, false);
                CardSlotManager.instance.deleteButton.localEulerAngles = new Vector3(0, 180f, 0);
                CardSlotManager.instance.deleteButton.localPosition = Vector3.zero;
            });
        }
        else
        {
            select.SetActive(false);
            CardSlotManager.instance.useButton.gameObject.SetActive(false);
            CardSlotManager.instance.deleteButton.gameObject.SetActive(false);
            rect.DOKill();
            transform.DOKill();

            rect.DOAnchorPos(originalLocalPos, 0.18f).SetEase(Ease.OutQuad);
            transform.DOScale(originalScale, 0.18f).SetEase(Ease.OutBack);
        }

    }
}
