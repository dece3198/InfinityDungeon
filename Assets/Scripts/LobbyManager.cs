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
        { Rank.S, 3 }, { Rank.A, 2 }, { Rank.B, 1 },
        { Rank.C, 1 }, { Rank.D, 0 }, { Rank.E, 0 }, { Rank.F, 0 }
    };
}


public class LobbyManager : Singleton<LobbyManager>
{
    [SerializeField] private GameObject subCam;
    [SerializeField] private RectTransform fadeInOut;
    public Transform[] roads;
    public Transform sitPos;
    public Transform paperPos;
    private Vector2 orignPos;
    private bool isCheck = false;

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

    private void Start()
    {
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

    public void PaperSetting(Mercenary mercenary)
    {
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


        if (mercenary.isCharacterImageFake)
        {
            mercenaryImage.sprite = GetRandomFakeImage(mercenary, charactorImage);
        }
        else
        {
            mercenaryImage.sprite = mercenary.mercenaryImage;
        }

        if(mercenary.isCoatingImageFake)
        {
            coatingImage.sprite = GetRandomFakeImage(mercenary, fakeCoatingImages);
        }
        else
        {
            coatingImage.sprite = realCoatingImage;
        }

        if(mercenary.mercenaryRank >= Rank.A)
        {
            AddSlot(mercenary.insignia);
        }
        
        if(mercenary.isInsigniaFake)
        {
            int randCount = Random.Range(1, 4);

            List<Insignia> pool = new List<Insignia>(fakeInsignias);

            
            for(int i = 0; i < randCount && pool.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, pool.Count);
                AddSlot(fakeInsignias[randomIndex]);
                pool.RemoveAt(randomIndex);
            }
        }
        else
        {
            if(mercenary.mercenaryRank >= Rank.C)
            {
                int maxCount = StatTable.insignia[mercenary.mercenaryRank];
                int randCount = Random.Range(0, maxCount + 1);
                AddRandomInsignia(mercenary, insignias, randCount);
            }
        }
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

            mercenary.atk = chosen.atk;
            mercenary.def = chosen.def;
            mercenary.hp = chosen.hp;

            AddSlot(chosen);
            candidates.RemoveAt(randomIndex);
        }
    }

    private void AddSlot(Insignia insignia)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].insignia == null)
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

    public void CheckButton()
    {
        isCheck = !isCheck;

        subCam.SetActive(isCheck);
    }
}
