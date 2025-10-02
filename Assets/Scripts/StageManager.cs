using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public Stage[] stage;
    public Transform[] startPos;
    public bool isStage = false;
    public int stageIndex;
    [SerializeField] private TextMeshProUGUI startText;
    private Vector3 originPos;

    private new void Awake()
    {
        base.Awake();
        originPos = startText.transform.localScale;
    }

    private void Start()
    {
        MonsterSetting();
    }

    public void MonsterSetting()
    {
        for(int i = 0; i < stage[stageIndex].monsterSpawns.Length; i++)
        {
            MonsterSpawn spawn = stage[stageIndex].monsterSpawns[i];
            GameObject monster = Instantiate(spawn.monsterPrefab, startPos[spawn.index]);
            monster.transform.localPosition = Vector3.zero;
            monster.SetActive(true);
        }
    }

    public void StageStart()
    {
        StartCoroutine(StartCo());
    }

    private IEnumerator StartCo()
    {
        startText.gameObject.SetActive(true);
        startText.text = "3";
        startText.transform.localScale = originPos;
        startText.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(1f);
        startText.text = "2";
        startText.transform.localScale = originPos;
        startText.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(1f);
        startText.text = "1";
        startText.transform.localScale = originPos;
        startText.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(1f);
        startText.text = "Start";
        startText.transform.localScale = originPos;
        startText.transform.DOScale(Vector3.one, 0.5f);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);
        isStage = true;
    }
}
