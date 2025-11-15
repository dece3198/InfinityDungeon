using UnityEngine;

[CreateAssetMenu(fileName = "UseCard", menuName = "New UseCard/UseCard")]
public class UseCard : ScriptableObject
{
    public int level;
    public string cardName;
    public Sprite cardImage;
    public UseType useType;
    public CardRank cardRank;
    public StageState stageState;
    public Mercenary mercenary;
    public bool isBot;
    public GameObject botPrefab;
    public GameObject LegendPrefab;
}
