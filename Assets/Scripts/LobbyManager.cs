using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StatTable
{
    public static readonly Dictionary<Rank, int> Age = new()
    {
        { Rank.S, 20 }, { Rank.A, 25 }, { Rank.B, 30 },
        { Rank.C, 35 }, { Rank.D, 40 }, { Rank.E, 45 }, { Rank.F, 50 }
    };

    public static readonly Dictionary<Rank, int> ManHeight = new()
    {
        { Rank.S, 190 }, { Rank.A, 185 }, { Rank.B, 180 },
        { Rank.C, 175 }, { Rank.D, 170 }, { Rank.E, 165 }, { Rank.F, 160 }
    };

    public static readonly Dictionary<Rank, int> WomanHeight = new()
    {
        { Rank.S, 175 }, { Rank.A, 170 }, { Rank.B, 165 },
        { Rank.C, 160 }, { Rank.D, 155 }, { Rank.E, 150 }, { Rank.F, 145 }
    };

    public static readonly Dictionary<Rank, int> ManWeightOffset = new()
    {
        { Rank.S, 0 }, { Rank.A, 8 }, { Rank.B, 16 },
        { Rank.C, 24 }, { Rank.D, 32 }, { Rank.E, 40 }, { Rank.F, 48 }
    };

    public static readonly Dictionary<Rank, int> WomanWeightOffset = new()
    {
        { Rank.S, 0 }, { Rank.A, 5 }, { Rank.B, 10 },
        { Rank.C, 15 }, { Rank.D, 20 }, { Rank.E, 25 }, { Rank.F, 30 }
    };

    public static readonly Dictionary<Rank, int> Insignia = new()
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
    //=== 데이터 및 상태 관리 변수 ===
    public MercenaryController curMercenary;
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
    [SerializeField] private int recruitmentIndex;
    public int RecruitmentIndex
    {
        get { return recruitmentIndex; }
        set 
        { 
            recruitmentIndex = value;
            recruiText.text = recruitmentIndex.ToString() + " / 5";
            if(recruitmentIndex == 5)
            {
                StartCoroutine(RecruitAllMercenariesCo());
            }
        }
    }
    // === Ui 참조 === 
    [Header("Game Display")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI recruiText;
    [SerializeField] private RectTransform fadeInOut;

    [Header("Game State")]
    public Transform[] roads;
    public Transform sitPos;
    public Transform paperPos;
    public bool isRecruit = true;

    public LobbyUiManager uiManager;

    [SerializeField] private GameObject subCam;
    [SerializeField] private GameObject dictionaryUi;
    [SerializeField] private GameObject checkUi;
    [SerializeField] private TextMeshProUGUI checkText;
    public GameObject recruitmentButton;

    [SerializeField] private Transform[] randerPos;
    [SerializeField] private MercenarySlot[] randerUi;

    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject dungeonButton;

    private int randerIndex = 0;
    private int checkIndex;
    private Vector2 orignPos;
    private bool isCheck = false;
    private bool isDictionary = false;
    private bool isConversion = false;

    private void Start()
    {
        Gold += 500;
        RecruitmentIndex = 0;
        orignPos = new Vector2(5000, 5000);
        fadeInOut.sizeDelta = Vector2.zero;

        for(int i = 0; i < randerUi.Length; i++)
        {
            randerUi[i].gameObject.SetActive(false);
        }
        fadeInOut.DOSizeDelta(orignPos, 1f);
    }

    //종이 세팅
    public void PaperSetting()
    {
        if(curMercenary != null)
        {
            uiManager.PaperSetting(curMercenary);
        }
    }

    //카메라 시점 전환 버튼
    public void ConversionButton()
    {
        isConversion = !isConversion;

        subCam.SetActive(isConversion);
    }

    //휘장 종류Ui 키고 끄기
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

    //버튼 눌렀을때 중요한 버튼 재확인 버튼
    public void CheckUi(int index)
    {
        if (index == 0 && !recruitmentButton.activeInHierarchy) return;

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

    //재확인 버튼 수락했을때 알맞는 Index로 작동하도록
    public void IndexButton()
    {
        isCheck = false;
        checkUi.SetActive(false);
        if(curMercenary != null)
        {
            switch (checkIndex)
            {
                case 0: Recruit(); break;
                case 1: uiManager.StatsCheck(curMercenary.mercenary); break;
            }
        }
    }

    //용병 영입
    private void Recruit()
    {
        if(Gold >= StatTable.Price[curMercenary.mercenary.mercenaryRank])
        {
            if (RecruitmentIndex < 5)
            {
                if (isRecruit && curMercenary != null)
                {
                    if (curMercenary.mercenaryState == MercenaryState.Sit)
                    {
                        StartCoroutine(RecruitCo());
                    }
                }
            }
        }
        else
        {
            MercenarySpawner.instance.NextMercenary();
        }
    }

    public void RanderTextureMercenary(MercenaryController mercenary)
    {
        mercenary.ChangeState(MercenaryState.Idle);
        mercenary.transform.localRotation = randerPos[randerIndex].rotation;
        mercenary.transform.position = randerPos[randerIndex].position;
        randerUi[randerIndex].gameObject.SetActive(true);
        randerUi[randerIndex].AddMercenary(mercenary.mercenary);
        randerIndex++;
    }

    public void DungeonButton()
    {
        StartCoroutine(FadeIn());
    }

    //용병 영입 코루틴
    private IEnumerator RecruitCo()
    {
        isRecruit = false;
        RecruitmentIndex++;
        uiManager.paper.SetActive(false);
        recruitmentButton.SetActive(false);
        MercenarySpawner.instance.curMercenary = null;
        Gold -= StatTable.Price[curMercenary.mercenary.mercenaryRank];
        curMercenary.posIndex = 8;
        curMercenary.animator.SetTrigger("Up");
        yield return new WaitForSeconds(2f);
        curMercenary.ChangeState(MercenaryState.Walk);
        curMercenary = null;    
        isRecruit = true;
    }

    //FadeOut
    private IEnumerator FadeOut()
    {
        fadeInOut.gameObject.SetActive(true);
        fadeInOut.DOSizeDelta(orignPos, 1f);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FadeIn()
    {
        fadeInOut.DOSizeDelta(Vector2.zero, 1f);
        yield return new WaitForSeconds(1.5f);
        AsyncOperation op = SceneManager.LoadSceneAsync("DungeonScene");
    }

    private IEnumerator RecruitAllMercenariesCo()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
        subCam.SetActive(false);
        yield return new WaitForSeconds(5f);
        dungeonButton.SetActive(true);
        uiManager.MercenaryUiButton();
    }
}
