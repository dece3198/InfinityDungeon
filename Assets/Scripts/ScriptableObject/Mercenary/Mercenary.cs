using UnityEngine;

public enum Rank
{
    F,E,D,C,B,A,S
}

public enum MercenaryClass
{
    SwordMan, Shieldbearer, Archer, Wizard, Assassin
}

[CreateAssetMenu(fileName = "New Mercenary", menuName = "New Mercenary/Mercenary")]
public class Mercenary : ScriptableObject
{
    [Header("Basic information")]
    public string mercenaryName;
    public Sprite mercenaryImage;
    public MercenaryClass mercenaryClass;
    public Rank ageRank;
    public Rank heightRank;
    public Rank weightRank;
    public Rank weaponRank;
    public Rank armorRank;
    public Rank potentialityRank;
    public Rank mercenaryRank;
    public int rankValu;

    [Header("Stats")]
    public float atk;
    public float def;
    public float speed;
    public float hp;
    public float criticalPercent;
    public float criticalDamage;
    public float skillDamage;

    [Header("Fake Information")]
    public bool isCharacterImageFake;
    public bool isCoatingImageFake;
    public bool isInsigniaFake;

    public Insignia insignia;

    public void CalculateStats()
    {
        rankValu = 0;
        ageRank = MixRank();
        heightRank = MixRank();
        weightRank = MixRank();
        weaponRank = MixRank();
        armorRank = MixRank();
        potentialityRank = MixRank();

        switch (mercenaryClass)
        {
            case MercenaryClass.SwordMan :
                atk = 23f; def = 15; speed = 1; hp = 100; skillDamage = 1.5f; criticalPercent = 0.25f; criticalDamage = 2f;
                break;
            case MercenaryClass.Shieldbearer:
                atk = 10f; def = 25; speed = 1; hp = 200; skillDamage = 1.5f; criticalPercent = 0.25f; criticalDamage = 2f;
                break;
            case MercenaryClass.Archer:
                atk = 7f; def = 10; speed = 2; hp = 50; skillDamage = 1.5f; criticalPercent = 0.5f; criticalDamage = 3f;
                break;
            case MercenaryClass.Wizard:
                atk = 5; def = 10; speed = 1; hp = 50; skillDamage = 3f; criticalPercent = 0.1f; criticalDamage = 2f;
                break;
            case MercenaryClass.Assassin:
                atk = 5; def = 20; speed = 2; hp = 50; skillDamage = 2f; criticalPercent = 0.75f; criticalDamage = 4f;
                break;
        }

        float RankValue(Rank r)
        {
            switch(r)
            {
                case Rank.F: return 0f;
                case Rank.E: return 0.05f;
                case Rank.D: return 0.1f;
                case Rank.C: return 0.15f;
                case Rank.B: return 0.2f;
                case Rank.A: return 0.25f;
                case Rank.S: return 0.3f;
                default: return 0f;
            }
        }

        float allBonus = RankValue(ageRank) + RankValue(heightRank);

        def -= def * RankValue(weightRank);
         
        atk += atk * (RankValue(weaponRank) * 1.5f);
        def += def * (RankValue(armorRank) * 1.5f);
        hp += hp * (RankValue(armorRank) * 2f);

        atk += atk * allBonus;
        hp += hp * allBonus;

        float rankMultiplier = 1 + (RankValue(potentialityRank) * 3f);
        atk *= rankMultiplier;
        def *= rankMultiplier;
        hp *= rankMultiplier;

        speed += (RankValue(ageRank) * 6f);
        speed += (RankValue(potentialityRank) * 6f);
        speed += (RankValue(weightRank) * 10f);
        speed = Mathf.Clamp(speed, 1f, 10f);

        mercenaryRank = RankJudgment();
    }

    private Rank RankJudgment()
    {
        if (rankValu >= 30) return Rank.S;
        if (rankValu >= 25) return Rank.A;
        if (rankValu >= 20) return Rank.B;
        if (rankValu >= 15) return Rank.C;
        if (rankValu >= 10) return Rank.D;
        if (rankValu >= 5) return Rank.E;
        return Rank.F;
    }

    public void RoundStats()
    {
        atk = Mathf.Round(atk);
        def = Mathf.Round(def);
        speed = Mathf.Round(speed);
        hp = Mathf.Round(hp);
    }

    private Rank MixRank()
    {
        int rand = Random.Range(0, 7);
        rankValu += rand;
        return (Rank)rand;
    }
}
