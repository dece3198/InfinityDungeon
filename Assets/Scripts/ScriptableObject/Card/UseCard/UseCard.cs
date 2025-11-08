using UnityEngine;

[CreateAssetMenu(fileName = "UseCard", menuName = "New UseCard/UseCard")]
public class UseCard : ScriptableObject
{
    public int level;
    public string cardName;
    public Sprite cardImage;
    public UseType useType;
    public CardRank cardRank;
}
