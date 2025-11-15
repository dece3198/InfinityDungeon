using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField] private int levelIndex;
    public int LevelIndex
    {
        get { return levelIndex; }
        set 
        { 
            levelIndex = value;
            levelIndexText.text = levelIndex.ToString() + " / " + Level.ToString();
        }
    }
    [SerializeField] private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            levelText.text = "Lv" + level.ToString();
        }
    }
    [SerializeField] private float exp;
    public float Exp
    {
        get { return exp; }
        set 
        { 
            exp = value;
            if(exp >= 100)
            {
                exp -= 100;
                Level++;
                levelIndexText.text = LevelIndex.ToString() + " / " + Level.ToString();
            }
            expBar.value = Mathf.Clamp01(exp / 100f);
        }
    }
    [SerializeField] private int gold;
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            goldText.text = gold.ToString() + "G";
        }
    }
    public List<GameObject> curUnits = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI levelIndexText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Slider expBar;
    [SerializeField] private UnitSlot[] waitSlot;
    [SerializeField] private Material fadeInOut;

    private void Start()
    {
        Level = 3;
        LevelIndex = 0;
        Gold = 50;
        for(int i = 0; i < GameManager.instance.mercenaryList.Count; i++)
        {
            GameObject unit = Instantiate(GameManager.instance.mercenaryList[i].unitPrefab);
            AddCard(unit);
            AddUnit(unit.GetComponent<UnitController>());
        }

        StartCoroutine(FadeOut());
    }

    private void AddCard(GameObject unit)
    {
        curUnits.Add(unit);
        if(unit.TryGetComponent(out UnitController controller))
        {
            foreach(var card in controller.mercenary.cards)
            {
                CardManager.instance.cards.Add(card);
            }
        }
    }

    public void UpGradeUnit(Card card)
    {
        for (int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i].TryGetComponent(out UnitController controller))
            {
                if (card.mercenary == controller.mercenary)
                {
                    switch (card.cardType)
                    {
                        case CardType.UnitAtk : controller.mercenary.atk += card.value; break;
                        case CardType.UnitDef: controller.mercenary.def += card.value; break;
                        case CardType.UnitHp: controller.mercenary.hp += card.value; break;
                    }
                    if (controller.gameObject.activeInHierarchy)
                    {
                        controller.upEffect.Play();
                    }
                    controller.AddStats();
                }
            }
        }
    }

    public void AllUnitUpGrade(Card card)
    {
        for(int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i].TryGetComponent(out UnitController controller))
            {
                switch (card.cardType)
                {
                    case CardType.AllAtk: controller.mercenary.atk += card.value; break;
                    case CardType.AllDef: controller.mercenary.def += card.value; break;
                    case CardType.AllHp: controller.mercenary.hp += card.value; break;
                }
                if (controller.gameObject.activeInHierarchy)
                {
                    controller.upEffect.Play();
                }
                controller.AddStats();
            }
        }
    }

    public void RandomUnitUpGrade(UseCard card)
    {
        int rand = Random.Range(0, curUnits.Count);
        curUnits[rand].GetComponent<UnitController>().Buff(card);
    }

    public void RestUnit()
    {
        for(int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i].TryGetComponent(out UnitController controller))
            {
                controller.AddStats();
                controller.ResetUnit();
            }
        }
    }
    
    public void GameStart()
    {
        for(int i = 0; i < curUnits.Count; i++)
        {
            if (curUnits[i].TryGetComponent(out Rigidbody r))
            {
                r.isKinematic = false;
            }
        }
    }

    public void AddUnit(UnitController unit)
    {
        for(int i = 0; i < waitSlot.Length; i++)
        {
            if(waitSlot[i].controller == null)
            {
                waitSlot[i].AddUnit(unit);
                return;
            }
        }
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            fadeInOut.SetFloat("_Fade", time);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float time = 1f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            fadeInOut.SetFloat("_Fade", time);
            yield return null;
        }
    }
}
