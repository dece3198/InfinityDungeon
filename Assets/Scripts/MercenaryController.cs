using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MercenaryState
{
    Walk, Give, Sit
}

public class MercenaryWalk : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        state.animator.SetBool("Move", true);
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
            if(state.posIndex == 4)
            {
                state.ChangeState(MercenaryState.Give);
            }
            else if(state.posIndex == 5)
            {
                state.ChangeState(MercenaryState.Sit);
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
        state.paper.transform.parent = LobbyManager.instance.paperPos;
        state.paper.transform.DOMove(LobbyManager.instance.paperPos.position, 0.25f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);
        state.paper.SetActive(false);
        LobbyManager.instance.paper.SetActive(true);
        yield return new WaitForSeconds(1f);
        state.posIndex++;
        state.ChangeState(MercenaryState.Walk);
    }
}

public class MercenarySit : BaseState<MercenaryController>
{
    public override void Enter(MercenaryController state)
    {
        state.animator.SetBool("Move", false);
        state.animator.SetTrigger("Sit");

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
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public float maxHp;
    public float atk;
    public float def;
    public float atkSpeed;
    public float speed;


    public Mercenary mercenary;
    public Animator animator;
    public Rigidbody rigid;
    private StateMachine<MercenaryState, MercenaryController> stateMachine = new StateMachine<MercenaryState, MercenaryController>();
    public int posIndex = 0;
    public GameObject paper;
    public MercenaryState mercenaryState;
    private Vector3 orignPos;

    public bool isMove;
    public bool isWait;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        stateMachine.Reset(this);
        stateMachine.AddState(MercenaryState.Walk, new MercenaryWalk());
        stateMachine.AddState(MercenaryState.Give, new MercenaryGive());
        stateMachine.AddState(MercenaryState.Sit, new MercenarySit());
        ChangeState(MercenaryState.Walk);
    }

    private void OnEnable()
    {
        mercenary.isCharacterImageFake = Random.value < 0.3f;
        mercenary.isCoatingImageFake = Random.value < 0.3f;
        mercenary.isInsigniaFake = Random.value < 0.3f;

        mercenary.CalculateStats();

        ApplyFakeStatModifier();

        LobbyManager.instance.PaperSetting(mercenary);

        atk = mercenary.atk;
        def = mercenary.def;
        Hp = mercenary.hp;
        maxHp = Hp;
        speed = mercenary.speed;
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

    public void OnSelect()
    {
        //잡을때 효과
        orignPos = transform.position;
    }

    public void FollowMouse(Vector3 targetPos)
    {
        //마우스 따라가기
        rigid.linearVelocity = Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }

    public void OnRelease()
    {
        //마우스 뗏을때
        if (MouseController.instance.curSlot != null)
        {
            transform.position = MouseController.instance.curSlot.transform.position;
            if (MouseController.instance.curSlot.slotType == UnitSlotType.Waiting)
            {
                gameObject.layer = 0;
                isWait = false;
            }
            else
            {
                gameObject.layer = 7;
                isWait = true;
            }
        }
        else
        {
            transform.position = orignPos;
        }
    }

    public void ChangeState(MercenaryState state)
    {
        stateMachine.ChangeState(state);
        mercenaryState = state;
    }
}
