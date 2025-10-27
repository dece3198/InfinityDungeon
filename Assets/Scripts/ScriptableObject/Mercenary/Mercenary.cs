using UnityEngine;

public enum Rank
{
    F,E,D,C,B,A,S
}

public enum MercenaryClass
{
    SwordMan, Shielder, Archer, FocusWizard, Assassin, Mauler, Lancer, Berserker, Hunter, Warrior, Thrower, Tanker, Healer, AreaWizard
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
    public float atkSpeed;
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

    public GameObject unitPrefab;

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
                atk = 23f; atkSpeed = 1.5f; def = 15; speed = 1; hp = 100; skillDamage = 4f; criticalPercent = 0.25f; criticalDamage = 2f;
                break;
            case MercenaryClass.Shielder:
                atk = 10f; atkSpeed = 2f; def = 25; speed = 1; hp = 200; skillDamage = 20f; criticalPercent = 0.25f; criticalDamage = 2f;
                break;
            case MercenaryClass.Archer:
                atk = 10f; atkSpeed = 1.5f; def = 10; speed = 2; hp = 50; skillDamage = 5f; criticalPercent = 0.5f; criticalDamage = 2f;
                break;
            case MercenaryClass.FocusWizard:
                atk = 5; atkSpeed = 2f; def = 10; speed = 1; hp = 50; skillDamage = 30f; criticalPercent = 0.1f; criticalDamage = 2f;
                break;
            case MercenaryClass.Assassin:
                atk = 7; atkSpeed = 0.5f; def = 20; speed = 2; hp = 50; skillDamage = 4f; criticalPercent = 0.75f; criticalDamage = 4f;
                break;
            case MercenaryClass.Mauler:
                atk = 25; atkSpeed = 3f;  def = 1; speed = 1; hp = 400; skillDamage = 5f; criticalPercent = 0.5f; criticalDamage = 2f;
                break;
            case MercenaryClass.Lancer:
                atk = 20; atkSpeed = 1.5f; def = 5; speed = 1; hp = 150; skillDamage = 1f; criticalPercent = 0.35f; criticalDamage = 3f;
                break;
            case MercenaryClass.Berserker:
                atk = 50; atkSpeed = 2f; def = 20; speed = 1; hp = 50; skillDamage = 4f; criticalPercent = 0.25f; criticalDamage = 2f;
                break;
            case MercenaryClass.Hunter:
                atk = 7; atkSpeed = 0.5f; def = 10; speed = 1; hp = 50; skillDamage = 4f; criticalPercent = 0.75f; criticalDamage = 4f;
                break;
            case MercenaryClass.Warrior:
                atk = 7; atkSpeed = 1f; def = 10; speed = 1; hp = 100; skillDamage = 4f; criticalPercent = 0.35f; criticalDamage = 4f;
                break;
            case MercenaryClass.Thrower:
                atk = 7; atkSpeed = 0.5f; def = 10; speed = 1; hp = 50; skillDamage = 4f; criticalPercent = 0.75f; criticalDamage = 4f;
                break;
            case MercenaryClass.Tanker:
                atk = 10; atkSpeed = 2f; def = 10; speed = 1; hp = 500; skillDamage = 4f; criticalPercent = 0.75f; criticalDamage = 2f;
                break;
            case MercenaryClass.Healer:
                atk = 20; atkSpeed = 2f; def = 5; speed = 1; hp = 50; skillDamage = 4f; criticalPercent = 0f; criticalDamage = 0f;
                break;
            case MercenaryClass.AreaWizard:
                atk = 10; atkSpeed = 1f; def = 10; speed = 1; hp = 50; skillDamage = 10f; criticalPercent = 0.5f; criticalDamage = 2f;
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

    public (float atk, float atkSpeed, float def, float hp, float speed, float critRate, float critDmg, float skillDmg) GetBaseStats()
    {
        switch (mercenaryClass)
        {
            case MercenaryClass.SwordMan: return (23f, 1.5f, 15f, 100f, 1f, 0.25f, 2f, 4f);
            case MercenaryClass.Shielder: return (10f, 2f, 25f, 200f, 1f, 0.25f, 2f, 20f);
            case MercenaryClass.Archer: return (10f, 1.5f, 10f, 50f, 2f, 0.5f, 2f, 5f);
            case MercenaryClass.FocusWizard: return (5f, 2f, 10f, 50f, 1f, 0.1f, 2f, 30f);
            case MercenaryClass.Assassin: return (7f, 0.5f, 20f, 50f, 2f, 0.75f, 4f, 4f);
            case MercenaryClass.Mauler: return (25f, 3f, 1f, 400f, 1f, 0.5f, 2f, 5f);
            case MercenaryClass.Lancer: return (20f, 1.5f, 5f, 150f, 1f, 0.35f, 3f, 1f);
            case MercenaryClass.Berserker: return (50f, 2f, 20f, 50f, 1f, 0.25f, 2f, 4f);
            case MercenaryClass.Hunter: return (7f, 0.5f, 10f, 50f, 1f, 0.75f, 4f, 4f);
            case MercenaryClass.Warrior: return (7f, 1f, 10f, 100f, 1f, 0.35f, 4f, 4f);
            case MercenaryClass.Thrower: return (7f, 0.5f, 10f, 50f, 1f, 0.75f, 4f, 4f);
            case MercenaryClass.Tanker: return (10f, 2f, 10f, 500f, 1f, 0.75f, 2f, 4f);
            case MercenaryClass.Healer: return (20f, 2f, 5f, 50f, 1f, 0f, 0f, 4f);
            case MercenaryClass.AreaWizard: return (10f, 1f, 10f, 50f, 1f, 0.5f, 2f, 10f);
            default: return (0, 0, 0, 0, 0, 0, 0, 0);
        }
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
