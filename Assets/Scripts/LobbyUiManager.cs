using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    //휘장
    [SerializeField] private Insignia[] insignias;
    [SerializeField] private Insignia[] fakeInsignias;

    //휘장 슬롯
    [SerializeField] private InsigniaSlot[] slots;

    //캐릭터, 승인도장 이미지
    [SerializeField] private Sprite[] charactorImage;
    [SerializeField] private Sprite[] fakeCoatingImages;
    [SerializeField] private Sprite realCoatingImage;

    //UIObj
    [SerializeField] private GameObject statsUi;
    [SerializeField] private GameObject curMercenaryUi;

    //용병 스탯 Ui Text
    [SerializeField] private TextMeshProUGUI[] statsTexts;
    [SerializeField] private TextMeshProUGUI[] baseStatsText;

    //용병이 들고 있는 증표와 보여질 증표Ui
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
    [SerializeField] private TextMeshProUGUI potentialityText;
    [SerializeField] private TextMeshProUGUI priceText;

    private bool isMercenary = false;

    //종이 세팅
    public void PaperSetting(MercenaryController mercenaryController)
    {

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        Mercenary mercenary = mercenaryController.mercenary;
        int rand = Random.Range(0, 3);
        int ageValue = StatTable.Age[mercenary.ageRank];
        int heightValue;
        float weightValue;
        if (mercenaryController.isMan)
        {
            heightValue = StatTable.ManHeight[mercenary.heightRank];
            weightValue = ((heightValue - 100) * 0.9f) + StatTable.ManWeightOffset[mercenary.weightRank];
        }
        else
        {
            heightValue = StatTable.WomanHeight[mercenary.heightRank];
            weightValue = ((heightValue - 100) * 0.9f) + StatTable.WomanWeightOffset[mercenary.weightRank];
        }
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
        potentialityText.text = $"{mercenary.potentialityRank.ToString()}";
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
                int maxCount = StatTable.Insignia[mercenary.mercenaryRank];
                int randCount = Random.Range(1, maxCount);
                AddRandomInsignia(mercenary, insignias, randCount);
            }
        }

        mercenary.RoundStats();
    }

    //가짜 용병이미지 랜덤 뽑기
    private Sprite GetRandomFakeImage(Mercenary mercenary, Sprite[] sprites)
    {
        List<Sprite> candidates = new List<Sprite>(sprites);
        candidates.Remove(mercenary.mercenaryImage);
        return candidates[Random.Range(0, candidates.Count)];
    }

    //휘장 랜덤 뽑기
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

    //랜덤 휘장 슬롯에 넣기
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

    //용병 스탯 확인
    public void StatsCheck(Mercenary mercenary)
    {
        LobbyManager.instance.Gold -= 3;
        statsUi.SetActive(true);

        var baseStats = mercenary.GetBaseStats();

        baseStatsText[0].text = baseStats.atk.ToString("N0");
        float safeAtkSpeed = Mathf.Max(0.1f, baseStats.atkSpeed);
        baseStatsText[1].text = ((1 / safeAtkSpeed) * 10).ToString("N0");
        baseStatsText[2].text = baseStats.def.ToString("N0");
        baseStatsText[3].text = baseStats.hp.ToString("N0");
        baseStatsText[4].text = (baseStats.critRate * 100).ToString("N0") + "%";
        baseStatsText[5].text = (baseStats.critDmg * 100).ToString("N0") + "%";
        baseStatsText[6].text = (baseStats.skillDmg * 100).ToString("N0") + "%";
        baseStatsText[7].text = baseStats.speed.ToString();


        StartCoroutine(ShowPowerCo(statsTexts[0], mercenary.atk, false));
        float safeMercAtkSpeed = Mathf.Max(0.1f, mercenary.atkSpeed);
        StartCoroutine(ShowPowerCo(statsTexts[1], (1 / safeMercAtkSpeed) * 10, false));
        StartCoroutine(ShowPowerCo(statsTexts[2], mercenary.def, false));
        StartCoroutine(ShowPowerCo(statsTexts[3], mercenary.hp, false));
        StartCoroutine(ShowPowerCo(statsTexts[4], mercenary.criticalPercent * 100, true));
        StartCoroutine(ShowPowerCo(statsTexts[5], mercenary.criticalDamage * 100, true));
        StartCoroutine(ShowPowerCo(statsTexts[6], mercenary.skillDamage * 100, true));
        StartCoroutine(ShowPowerCo(statsTexts[7], mercenary.speed, false));
    }
    
    public void StatsCheckButton()
    {
        statsUi.SetActive(false);
    }

    public void MercenaryUiButton()
    {
        isMercenary = !isMercenary;

        if (isMercenary)
        {
            curMercenaryUi.SetActive(true);
            curMercenaryUi.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
        }
        else
        {
            curMercenaryUi.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
            curMercenaryUi.SetActive(false);
        }
    }

    //스탯 정보 코루틴으로 연출
    private IEnumerator ShowPowerCo(TextMeshProUGUI text, float value, bool percent)
    {
        float time = 0f;
        float interval = 0.02f;

        while (time < 2)
        {
            float rand = Random.Range(0, value * 2);
            if (percent)
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

        if (percent)
        {
            text.text = value.ToString("N0") + "%";
        }
        else
        {
            text.text = value.ToString("N0");
        }
    }
}
