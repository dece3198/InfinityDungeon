using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum UseType
{
    MonsterAtk, MonsterDef, MonsterSpeed, MonsterStun, UnitAtk, UnitDef, UnitHp, UnitSpeed
}

public static class UseCardGenerator
{
    public static void CreateRandomCard(UseCard _card)
    {
        UseCard card = _card;

        float rand = Random.value * 100f;
        if (rand < 50f) card.cardRank = CardRank.Silver;
        else if (rand < 85f) card.cardRank = CardRank.Gold;
        else if (rand < 99f) card.cardRank = CardRank.Eapic;
        else card.cardRank = CardRank.Legend;

        switch (card.cardRank)
        {
            case CardRank.Silver: card.level = Random.Range(1, 15); break;
            case CardRank.Gold: card.level = Random.Range(15, 30); break;
            case CardRank.Eapic: card.level = Random.Range(30, 60); break;
            case CardRank.Legend: card.level = Random.Range(60, 99); break;
        }

    }
}

public class CardSlotManager : Singleton<CardSlotManager>
{
    [SerializeField] private List<RectTransform> cardList = new List<RectTransform>();
    [SerializeField] private UseCardSlot[] cards;
    [SerializeField] private Sprite[] rankImage;
    [SerializeField] private UseCard[] useCards;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform discardPos;
    [SerializeField] private GameObject takeButton;
    [SerializeField] private GameObject discardButton;
    [SerializeField] private RectTransform curCard;
    [SerializeField] private TextMeshProUGUI errorText;
    public Transform useButton;
    public Transform deleteButton;

    public void TakeCardButton()
    {
        if(cardList.Count < DungeonManager.instance.Level)
        {
            if (curCard != null)
            {
                takeButton.SetActive(false);
                discardButton.SetActive(false);
                curCard.transform.SetAsLastSibling();

                curCard.transform.DOScale(Vector3.one, 0.35f)
                    .SetEase(Ease.OutBack);

                curCard.transform.DOMove(transform.position, 0.45f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        cardList.Add(curCard);
                        curCard.GetComponent<UseCardSlot>().isUse = true;
                        RefreshCards();
                        curCard = null;
                    });
            }
        }
        else
        {
            errorText.text = "No available slots.";

            RectTransform rt = errorText.GetComponent<RectTransform>();

            rt.DOShakePosition(
                duration: 1f,      // Èçµé¸®´Â ½Ã°£
                strength: 10f,       // Èçµé¸± ¹üÀ§
                vibrato: 20,          // Èçµé¸² ¹Ýº¹ È½¼ö
                randomness: 90f,      // ·£´ý °­µµ
                snapping: false,
                fadeOut: true
            );

            errorText.DOFade(1f, 1f).OnComplete(() => errorText.DOFade(0f, 0.4f));
        }
    }

    public void DiscardButton()
    {
        if (curCard == null) return;

        takeButton.SetActive(false);
        discardButton.SetActive(false);
        curCard.DOKill();
        curCard.transform.SetAsLastSibling();

        Sequence seq = DOTween.Sequence();

        seq.Append(
            curCard.DOScale(Vector3.one * 1.15f, 0.15f)
            .SetEase(Ease.OutQuad)
        );

        seq.Append(
            curCard.DOMove(discardPos.position, 0.6f)
            .SetEase(Ease.InQuad)
        );

        seq.Join(
            curCard.DOLocalRotate(
                new Vector3(0, 0, Random.Range(-1440f, 1440f)),
                0.6f,
                RotateMode.FastBeyond360
            ).SetEase(Ease.Linear)
        );
        seq.Join(
            curCard.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutCubic)
        );

        seq.OnComplete(() =>
        {
            curCard.GetComponent<UseCardSlot>().ClearCard();
            curCard = null;
        });
    }

    public void RandomCard()
    {
        if(curCard  != null) return;

        if (DungeonManager.instance.Gold < 10) return;

        DungeonManager.instance.Gold -= 10;

        for(int i = 0; i < cards.Length; i++)
        {
            if(cards[i].card == null)
            {
                int rand = Random.Range(0, useCards.Length);
                UseCard tempCard = useCards[rand];
                UseCardGenerator.CreateRandomCard(tempCard);
                cards[i].AddCard(tempCard, rankImage[(int)tempCard.cardRank]);

                if(tempCard.cardRank == CardRank.Legend)
                {
                    cards[i].legendCard.gameObject.SetActive(true);
                    cards[i].legendCard.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
                }

                cards[i].transform.position = startPos.position;
                cards[i].gameObject.SetActive(true);
                cards[i].transform.DOMove(centerPos.position, 0.6f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    takeButton.SetActive(true);
                    discardButton.SetActive(true);
                });
                cards[i].transform.DOLocalRotate(new Vector3(0, cards[i].isFlip ? 0f : 180f, 0), 0.4f).SetEase(Ease.OutSine);
                cards[i].transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
                curCard = cards[i].GetComponent<RectTransform>();

                return;
            }
        }
    }

    public void RefreshCards()
    {
        int count = cardList.Count;
        if (count == 0) return;

        float minRadius = 250f;
        float maxRadius = 1000f;
        float minAngle = 10f;
        float maxAngle = 30f;

        float t = Mathf.InverseLerp(2, 9, count);
        float radius = Mathf.Lerp(minRadius, maxRadius, t);
        float angleRange = Mathf.Lerp(minAngle, maxAngle, t);

        if (count == 1)
        {
            RectTransform onlyCard = cardList[0];
            onlyCard.DOKill();
            onlyCard.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutQuad);
            onlyCard.DOLocalRotate(new Vector3(0, 180f, 0), 0.3f).SetEase(Ease.OutQuad);
            return;
        }


        float angleStep = (count > 1) ? (angleRange * 2f) / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = -angleRange + (angleStep * i);
            float rad = Mathf.Deg2Rad * angle;

            Vector2 targetPos = new Vector2(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius - radius);
            float rotZ = -angle * 0.7f;

            RectTransform card = cardList[i];
            card.DOKill();
            card.SetSiblingIndex(i);
            card.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.OutQuad);
            card.DOLocalRotate(new Vector3(0, 180f, -rotZ), 0.3f).SetEase(Ease.OutQuad);
        }
    }

}
