using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    UnitAtk, UnitDef, UnitHp, AllAtk, AllDef, AllHp, Level, Gold
}

public enum CardRank
{
    Silver, Gold, Eapic, Legend
}

public static class CardGenerator
{
    public static void CreateRandomCard(Card _card)
    {
        Card card = _card;

        float rand = Random.value * 100f;
        if (rand < 50f) card.cardRank = CardRank.Silver;
        else if (rand < 85f) card.cardRank = CardRank.Gold;
        else if (rand < 99f) card.cardRank = CardRank.Eapic;
        else card.cardRank = CardRank.Legend;

        switch(card.cardRank)
        {
            case CardRank.Silver: card.level = Random.Range(0, 15); break;
            case CardRank.Gold: card.level = Random.Range(15, 30); break;
            case CardRank.Eapic: card.level = Random.Range(30, 60); break;
            case CardRank.Legend: card.level = Random.Range(60, 100); break;
        }

        switch (card.cardType)
        {
            case CardType.UnitAtk:
                card.value = GetStatValue(card);
                break;
            case CardType.UnitDef:
                card.value = GetStatValue(card);
                break;
            case CardType.UnitHp:
                card.value = GetStatValue(card);
                break;
            case CardType.AllAtk:
                card.value = GetStatValue(card);
                break;
            case CardType.AllDef:
                card.value = GetStatValue(card);
                break;
            case CardType.AllHp:
                card.value = GetStatValue(card);
                break;
            case CardType.Level:
                card.value = GetExpValue(card.cardRank);
                break;
            case CardType.Gold:
                card.value = GetGoldValue(card.cardRank);
                break;
        }
    }

    private static int GetStatValue(Card _card)
    {
        float baseValue = _card.level;
        switch (_card.cardType)
        {
            case CardType.UnitAtk: baseValue *= 0.5f; break;
            case CardType.UnitDef: baseValue *= 0.3f; break;
            case CardType.UnitHp: baseValue *= 3f; break;
            case CardType.AllAtk: baseValue *= 0.2f; break;
            case CardType.AllDef: baseValue *= 0.1f; break;
            case CardType.AllHp: baseValue *= 1f; break;

        }
        return Mathf.RoundToInt(baseValue);
    }

    private static int GetExpValue(CardRank rank)
    {
        return rank switch
        {
            CardRank.Silver => 25,
            CardRank.Gold => 50,
            CardRank.Eapic => 75,
            CardRank.Legend => 100,
            _ => 0
        };
    }

    private static int GetGoldValue(CardRank rank)
    {
        return rank switch
        {
            CardRank.Silver => 5,
            CardRank.Gold => 10,
            CardRank.Eapic => 20,
            CardRank.Legend => 50,
            _ => 0
        };
    }
}

public class CardManager : Singleton<CardManager>   
{
    [SerializeField] private GameObject cardParent;
    [SerializeField] private CardSlot[] cardSlots;
    [SerializeField] private Sprite[] cardRankImages;
    public List<Card> cards = new List<Card>();

    public void ShuffleCard()
    {
        StartCoroutine(ShuffleCo());
    }

    private IEnumerator ShuffleCo()
    {
        List<Card> cardList = new List<Card>(cards);
        cardParent.SetActive(true);
        cardParent.transform.DOScale(Vector3.one, 0.25f);
        yield return new WaitForSeconds(0.25f);
        foreach (var card in cardSlots)
        {
            int rand = Random.Range(0, cardList.Count);
            Card tempCard = cardList[rand];
            CardGenerator.CreateRandomCard(tempCard);
            card.AddCard(tempCard, cardRankImages[(int)tempCard.cardRank]);
            card.CardFlip();
            cardList.Remove(tempCard);
            yield return new WaitForSeconds(1f);
        }
    }
}
