using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class StatTable
{
    public static readonly Dictionary<Rank, int> Age = new()
    {
        { Rank.S, 20 }, { Rank.A, 25 }, { Rank.B, 30 },
        { Rank.C, 35 }, { Rank.D, 40 }, { Rank.E, 45 }, { Rank.F, 50 }
    };

    public static readonly Dictionary<Rank, int> Height = new()
    {
        { Rank.S, 190 }, { Rank.A, 185 }, { Rank.B, 180 },
        { Rank.C, 175 }, { Rank.D, 170 }, { Rank.E, 165 }, { Rank.F, 160 }
    };

    public static readonly Dictionary<Rank, int> WeightOffset = new()
    {
        { Rank.S, 0 }, { Rank.A, 8 }, { Rank.B, 16 },
        { Rank.C, 24 }, { Rank.D, 32 }, { Rank.E, 40 }, { Rank.F, 48 }
    };

    public static readonly Dictionary<Rank, int> insignia = new()
    {
        { Rank.S, 4 }, { Rank.A, 3 }, { Rank.B, 2 },
        { Rank.C, 1 }, { Rank.D, 0 }, { Rank.E, 0 }, { Rank.F, 0 }
    };

    public static readonly Dictionary<Rank, int> Price = new()
    {
        { Rank.S, 200}, { Rank.A, 150}, { Rank.B, 100},
        { Rank.C, 50}, { Rank.D, 25 }, { Rank.E, 10 }, { Rank.F, 0 }
    };
}


public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private int gold;
    public int Gold
    {
        get { return gold; }
        set 
        {
            gold = value;
            goldText.text = gold.ToString();
        }
    }

    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject subCam;
    [SerializeField] private GameObject dictionaryUi;
    [SerializeField] private GameObject statsUi;
    [SerializeField] private RectTransform fadeInOut;
    [SerializeField] private GameObject checkUi;
    [SerializeField] private TextMeshProUGUI checkText;
    [SerializeField] private TextMeshProUGUI priceText;
    private int checkIndex;
    private int recruitmentIndex;
    public Transform[] roads;
    public Transform sitPos;
    public Transform paperPos;
    private Vector2 orignPos;
    private bool isCheck = false;
    private bool isDictionary = false;
    private bool isStats = false;
    private bool isConversion = false;
    public MercenaryController curMercenary;

    [SerializeField] private Insignia[] insignias;
    [SerializeField] private Insignia[] fakeInsignias;

    [SerializeField] private Sprite[] charactorImage;
    [SerializeField] private Sprite[] fakeCoatingImages;
    [SerializeField] private Sprite realCoatingImage;

    [SerializeField] private InsigniaSlot[] slots;

    public GameObject paper;
    [SerializeField] private Image mercenaryImage;
    [SerializeField] private Image coatingImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI nameTextUi;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI classTextUi;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI ageTextUi;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI rankTextUi;
    [SerializeField] private TextMeshProUGUI heightText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI weaponRankText;
    [SerializeField] private TextMeshProUGUI armorRankText;

    [SerializeField] private TextMeshProUGUI[] statsTexts;

    private void Start()
    {
        Gold += 500;
        orignPos = new Vector2(5000, 5000);
        fadeInOut.sizeDelta = Vector2.zero;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        fadeInOut.gameObject.SetActive(true);
        fadeInOut.DOSizeDelta(orignPos, 1f);
        yield return new WaitForSeconds(1f);
    }

    public void PaperSetting()
    {

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        Mercenary mercenary = curMercenary.mercenary;
        int rand = Random.Range(0, 3);
        int ageValue = StatTable.Age[mercenary.ageRank];
        int heightValue = StatTable.Height[mercenary.heightRank];
        float weightValue = ((heightValue - 100) * 0.9f) + StatTable.WeightOffset[mercenary.weightRank];

        string name = $"Name : {mercenary.mercenaryName}";
        string mercenaryClass = $"Class : {mercenary.mercenaryClass.ToString()}";
        string age = $"Age : {ageValue}";
        string rank = $"{mercenary.mercenaryRank.ToString()}";


        nameText.text = name;
        nameTextUi.text = name;
        classText.text = mercenaryClass;
        classTextUi.text = mercenaryClass;
        ageText.text = age;
        ageTextUi.text = age;
        rankText.text = rank;
        rankTextUi.text = rank;
        heightText.text = $"Height : {heightValue}cm";
        weightText.text = $"Weight : {weightValue}kg";
        weaponRankText.text = $"{mercenary.weaponRank.ToString()}";
        armorRankText.text = $"{mercenary.armorRank.ToString()}";
        priceText.text = $"Price : {StatTable.Price[mercenary.mercenaryRank]}Gold";

        if (mercenary.isCharacterImageFake)
        {
            mercenaryImage.sprite = GetRandomFakeImage(mercenary, charactorImage);
        }
        else
        {
            mercenaryImage.sprite = mercenary.mercenaryImage;
        }

        if (mercenary.isCoatingImageFake)
        {
            coatingImage.sprite = GetRandomFakeImage(mercenary, fakeCoatingImages);
        }
        else
        {
            coatingImage.sprite = realCoatingImage;
        }

        if (mercenary.mercenaryRank >= Rank.A)
        {
            AddSlot(mercenary.insignia);
        }

        if (mercenary.isInsigniaFake)
        {
            int randCount = Random.Range(1, 4);

            List<Insignia> pool = new List<Insignia>(fakeInsignias);


            for (int i = 0; i < randCount && pool.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, pool.Count);
                AddSlot(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }
        }
        else
        {
            if (mercenary.mercenaryRank >= Rank.C)
            {
                int maxCount = StatTable.insignia[mercenary.mercenaryRank];
                int randCount = Random.Range(1, maxCount);
                AddRandomInsignia(mercenary, insignias, randCount);
            }
        }

        mercenary.RoundStats();
    }

    private void AddRandomInsignia(Mercenary mercenary, Insignia[] pool, int count)
    {
        List<Insignia> candidates = new List<Insignia>(pool);

        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            Insignia chosen = candidates[randomIndex];

            if (chosen.insigniaRank > mercenary.mercenaryRank)
            {
                if (Random.value > 0.05f)
                {
                    candidates.RemoveAt(randomIndex);
                    i--;
                    continue;
                }
            }

            mercenary.atk += chosen.atk;
            mercenary.def += chosen.def;
            mercenary.hp += chosen.hp;

            AddSlot(chosen);
            candidates.RemoveAt(randomIndex);
        }
    }

    private void AddSlot(Insignia insignia)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].insignia == null)
            {
                slots[i].AddSlot(insignia);
                return;
            }
        }
    }

    private Sprite GetRandomFakeImage(Mercenary mercenary, Sprite[] sprites)
    {
        List<Sprite> candidates = new List<Sprite>(sprites);
        candidates.Remove(mercenary.mercenaryImage);
        return candidates[Random.Range(0, candidates.Count)];
    }

    public void ConversionButton()
    {
        isConversion = !isConversion;

        subCam.SetActive(isConversion);
    }

    public void DictionaryUiOpen()
    {
        isDictionary = !isDictionary;

        if (isDictionary)
        {
            dictionaryUi.SetActive(true);
            dictionaryUi.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
        }
        else
        {
            dictionaryUi.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
            dictionaryUi.SetActive(false);
        }
    }

    public void CheckUi(int index)
    {
        isCheck = !isCheck;

        if(isCheck)
        {
            checkUi.SetActive(true);
            switch (index)
            {
                case 0: checkText.text = "Would you like to recruit mercenaries?"; break;
                case 1: checkText.text = "Would you like to check the stats?"; break;
            }
            checkIndex = index;
        }
        else
        {
            checkUi.SetActive(false);
        }

    }

    public void IndexButton()
    {
        isCheck = false;
        checkUi.SetActive(false);
        switch (checkIndex)
        {
            case 0: Recruit(); break;
            case 1: StatsCheck(); break;
        }
    }

    private void StatsCheck()
    {
        isStats = !isStats;

        if (isStats)
        {
            Gold -= 3;
            statsUi.SetActive(true);
            StartCoroutine(ShowPowerCo(statsTexts[0], curMercenary.atk));
            StartCoroutine(ShowPowerCo(statsTexts[1], curMercenary.def));
            StartCoroutine(ShowPowerCo(statsTexts[2], curMercenary.Hp));
            StartCoroutine(ShowPowerCo(statsTexts[3], curMercenary.mercenary.speed));
            StartCoroutine(ShowPowerCo(statsTexts[4], curMercenary.criticalP * 100));
            StartCoroutine(ShowPowerCo(statsTexts[5], curMercenary.criticalD * 100));
        }
        else
        {
            statsUi.SetActive(false);
        }

    }

    private void Recruit()
    {
        recruitmentIndex++;
        Gold -= StatTable.Price[curMercenary.mercenary.mercenaryRank];
    }

    private IEnumerator ShowPowerCo(TextMeshProUGUI text, float value)
    {
        float time = 0f;
        float interval = 0.02f;

        while(time < 2)
        {
            float rand = Random.Range(0, value * 2);
            if (value == curMercenary.criticalP * 100 || value == curMercenary.criticalD * 100)
            {
                text.text = rand.ToString("N0") + "%";
            }
            else
            {
                text.text = rand.ToString("N0");
            }

            time += interval;
            interval = Mathf.Lerp(0.02f, 0.15f, time / 2);
            yield return new WaitForSeconds(interval);
        }

        if(value == curMercenary.criticalP * 100 ||  value == curMercenary.criticalD * 100)
        {
            text.text = value.ToString("N0") + "%";
        }
        else
        {
            text.text = value.ToString("N0");
        }
    }
}
