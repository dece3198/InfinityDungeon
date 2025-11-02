using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "New Card/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite unitImage;
    public Sprite typeImage;
    public int level;
    public int value;
    public CardType cardType;
    public CardRank cardRank;
    public Mercenary mercenary;
}
