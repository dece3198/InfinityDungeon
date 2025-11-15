using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum StageState
{
    Wait, Battle,End
}

public class WaitStage : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class BattleStage : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class EndStage : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class StageManager : Singleton<StageManager>
{
    public Stage[] stage;
    public Transform[] startPos;
    public List<MonsterController> monsterList = new List<MonsterController>();
    public int stageIndex;
    [SerializeField] private TextMeshProUGUI startText;
    public StageState stageState;
    private StateMachine<StageState, StageManager> stateMachine = new StateMachine<StageState, StageManager>();
    private Vector3 originPos;
    public bool isStage = false;
    private bool isStartStage = true;

    private new void Awake()
    {
        base.Awake();
        originPos = startText.transform.localScale;
        stateMachine.Reset(this);
        stateMachine.AddState(StageState.Wait, new WaitStage());
        stateMachine.AddState(StageState.Battle, new BattleStage());
        stateMachine.AddState(StageState.End, new EndStage());
        ChangeState(StageState.Wait);
    }

    private void Start()
    {
        MonsterSetting();
    }

    public void MonsterSetting()
    {
        for (int i = 0; i < stage[stageIndex].monsterSpawns.Length; i++)
        {
            MonsterSpawn spawn = stage[stageIndex].monsterSpawns[i];
            GameObject monster = Instantiate(spawn.monsterPrefab, startPos[spawn.index]);
            if (monster.TryGetComponent(out MonsterController m))
            {
                monsterList.Add(m);
            }
            monster.transform.localPosition = Vector3.zero;
            monster.SetActive(true);
        }
    }

    public void StageStart()
    {
        if (isStartStage)
        {
            StartCoroutine(StartCo());
        }
    }

    public void DeBuffMonster(UseCard card)
    {
        int rand = Random.Range(0, monsterList.Count);
        monsterList[rand].DeBuff(card);
    } 

    public void ChangeState(StageState state)
    {
        stateMachine.ChangeState(state);
        stageState = state;
    }

    private IEnumerator StartCo()
    {
        isStartStage = false;
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
        DungeonManager.instance.GameStart();
        isStage = true;
    }

}
