using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MercenaryState
{
    Idle,Walk, Give, Sit
}

public class MercenaryIdle : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        state.animator.SetBool("Move", false);
    }

    public override void Exit(MercenaryController state)
    {
    }

    public override void FixedUpdate(MercenaryController state)
    {
    }

    public override void Update(MercenaryController state)
    {
    }
}

public class MercenaryWalk : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        state.animator.SetBool("Move", true);
        state.rigid.isKinematic = false;
    }

    public override void Exit(MercenaryController state)
    {
    }

    public override void FixedUpdate(MercenaryController state)
    {
        Vector3 dir = (LobbyManager.instance.roads[state.posIndex].position - state.transform.position).normalized;
        dir.y = 0;
        Vector3 moveVec = dir * state.speed * Time.fixedDeltaTime;
        state.rigid.MovePosition(state.rigid.position + moveVec);
        state.rigid.linearVelocity = Vector3.zero;

        if(dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            state.transform.rotation = Quaternion.Slerp(state.transform.rotation, targetRot, 5f * Time.fixedDeltaTime);
        }


        float dist = (LobbyManager.instance.roads[state.posIndex].position - state.transform.position).sqrMagnitude;

        if(dist <= 0.1f)
        {
            if (state.posIndex == 4)
            {
                state.ChangeState(MercenaryState.Give);
            }
            else if (state.posIndex == 5)
            {
                state.ChangeState(MercenaryState.Sit);
            }
            else if (state.posIndex == 7)
            {
                state.posIndex = 0;
                state.paper.SetActive(true);
                MercenarySpawner.instance.EnterPool(state.gameObject);
            }
            else if(state.posIndex == 8)
            {
                GameManager.instance.mercenaryList.Add(state.mercenary);
                LobbyManager.instance.RanderTextureMercenary(state);
            }
            else
            {
                state.posIndex++;
            }
        }
    }

    public override void Update(MercenaryController state)
    {
    }
}

public class MercenaryGive : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        state.animator.SetTrigger("Give");
        state.animator.SetBool("Move", false);

        Vector3 dir = (LobbyManager.instance.roads[state.posIndex].forward).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        state.StartCoroutine(RotateToTarget(state, targetRot, 5f));
    }

    public override void Exit(MercenaryController state)
    {
    }

    public override void FixedUpdate(MercenaryController state)
    {
    }

    public override void Update(MercenaryController state)
    {
    }

    private IEnumerator RotateToTarget(MercenaryController state, Quaternion targetRot, float speed)
    {
        while (Quaternion.Angle(state.transform.rotation, targetRot) > 0.5f)
        {
            state.transform.rotation = Quaternion.Slerp(
                state.transform.rotation,
                targetRot,
                speed * Time.deltaTime
            );
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        state.paper.transform.DOMove(LobbyManager.instance.paperPos.position, 0.25f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);
        state.paper.transform.position = state.paperPos.position;
        state.paper.transform.rotation = state.paperPos.rotation;
        state.paper.SetActive(false);
        LobbyManager.instance.uiManager.paper.SetActive(true);
        yield return new WaitForSeconds(1f);
        state.posIndex++;
        state.ChangeState(MercenaryState.Walk);
    }
}

public class MercenarySit : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        MercenarySpawner.instance.next.SetActive(true);
        state.animator.SetBool("Move", false);
        state.animator.SetTrigger("Sit");
        LobbyManager.instance.recruitmentButton.SetActive(true);
        Vector3 dir = (LobbyManager.instance.roads[state.posIndex].forward).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        state.rigid.isKinematic = true;
        state.transform.position = LobbyManager.instance.sitPos.position;
        state.StartCoroutine(RotateToTarget(state, targetRot, 3f));
    }

    public override void Exit(MercenaryController state)
    {
    }

    public override void FixedUpdate(MercenaryController state)
    {
    }

    public override void Update(MercenaryController state)
    {
    }

    private IEnumerator RotateToTarget(MercenaryController state, Quaternion targetRot, float speed)
    {
        while (Quaternion.Angle(state.transform.rotation, targetRot) > 0.5f)
        {
            state.transform.rotation = Quaternion.Slerp(
                state.transform.rotation,
                targetRot,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }
}

public class MercenaryController : MonoBehaviour
{
    public float speed;
    public Mercenary mercenary;
    public Animator animator;
    public Rigidbody rigid;
    private StateMachine<MercenaryState, MercenaryController> stateMachine = new StateMachine<MercenaryState, MercenaryController>();
    public int posIndex = 0;
    public GameObject paper;
    public MercenaryState mercenaryState;
    private Vector3 orignPos;
    public Transform paperPos;
    public bool isMan;
    public bool isMove;
    public bool isWait;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        stateMachine.Reset(this);
        stateMachine.AddState(MercenaryState.Idle, new MercenaryIdle());
        stateMachine.AddState(MercenaryState.Walk, new MercenaryWalk());
        stateMachine.AddState(MercenaryState.Give, new MercenaryGive());
        stateMachine.AddState(MercenaryState.Sit, new MercenarySit());
    }

    private void OnEnable()
    {
        LobbyManager.instance.curMercenary = this;
        paper.SetActive(true);
        ChangeState(MercenaryState.Walk);
        mercenary.isCharacterImageFake = Random.value < 0.1f;
        mercenary.isCoatingImageFake = Random.value < 0.2f;
        mercenary.isInsigniaFake = Random.value < 0.3f;

        mercenary.CalculateStats();

        LobbyManager.instance.PaperSetting();

        ApplyFakeStatModifier();

        if (isMan)
        {
            animator.SetFloat("Blend", 0);
        }
        else
        {
            animator.SetFloat("Blend", 1);
        }
    }

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void ApplyFakeStatModifier()
    {
        // 위조 종류별 능력치 감소율
        float photoPenalty = mercenary.isCharacterImageFake ? 0.7f : 1f;   // 30% 감소
        float stampPenalty = mercenary.isCoatingImageFake ? 0.8f : 1f;     // 20% 감소
        float insigniaPenalty = mercenary.isInsigniaFake ? 0.9f : 1f;      // 10% 감소

        float totalPenalty = photoPenalty * stampPenalty * insigniaPenalty;

        // 전체 능력치 조정
        mercenary.atk *= totalPenalty;
        mercenary.def *= totalPenalty;
        mercenary.hp *= totalPenalty;
        mercenary.speed *= totalPenalty;

        mercenary.RoundStats();
    }

    public void ChangeState(MercenaryState state)
    {
        stateMachine.ChangeState(state);
        mercenaryState = state;
    }
}
