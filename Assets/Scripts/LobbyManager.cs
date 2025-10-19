using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    //=== ������ �� ���� ���� ���� ===
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
                //���� ���� ��ư Ȱ��ȭ
            }
        }
    }
    // === Ui ���� === 
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

    [SerializeField] private Transform[] randerPos;
    [SerializeField] private MercenarySlot[] randerUi;

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
        StartCoroutine(FadeOut());
    }

    //���� ����
    public void PaperSetting()
    {
        if(curMercenary != null)
        {
            uiManager.PaperSetting(curMercenary);
        }
    }

    //ī�޶� ���� ��ȯ ��ư
    public void ConversionButton()
    {
        isConversion = !isConversion;

        subCam.SetActive(isConversion);
    }

    //���� ����Ui Ű�� ����
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

    //��ư �������� �߿��� ��ư ��Ȯ�� ��ư
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

    //��Ȯ�� ��ư ���������� �˸´� Index�� �۵��ϵ���
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

    //�뺴 ����
    private void Recruit()
    {
        if(RecruitmentIndex < 5)
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

    public void RanderTextureMercenary(MercenaryController mercenary)
    {
        mercenary.ChangeState(MercenaryState.Idle);
        mercenary.transform.localRotation = randerPos[randerIndex].rotation;
        mercenary.transform.position = randerPos[randerIndex].position;
        randerUi[randerIndex].gameObject.SetActive(true);
        randerUi[randerIndex].AddMercenary(mercenary.mercenary);
        randerIndex++;
    }

    //FadeOut
    private IEnumerator FadeOut()
    {
        fadeInOut.gameObject.SetActive(true);
        fadeInOut.DOSizeDelta(orignPos, 1f);
        yield return new WaitForSeconds(1f);
    }

    //�뺴 ���� �ڷ�ƾ
    private IEnumerator RecruitCo()
    {
        isRecruit = false;
        RecruitmentIndex++;
        uiManager.paper.SetActive(false);
        MercenarySpawner.instance.curMercenary = null;
        Gold -= StatTable.Price[curMercenary.mercenary.mercenaryRank];
        curMercenary.posIndex = 8;
        curMercenary.animator.SetTrigger("Up");
        yield return new WaitForSeconds(2f);
        curMercenary.ChangeState(MercenaryState.Walk);
        curMercenary = null;    
        isRecruit = true;
    }

}
