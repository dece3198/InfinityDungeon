using UnityEngine;

[CreateAssetMenu(fileName = "New Insignia", menuName = "New Insignia/Insignia")]
public class Insignia : ScriptableObject
{
    public string insigniaName;
    public Sprite insigniaImage;
    public Rank insigniaRank;
    public float atk;
    public float def;
    public float hp;
    public float criticalPercent;
    public float criticalDamage;
}
