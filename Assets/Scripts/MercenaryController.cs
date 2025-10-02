using System.Collections;
using UnityEngine;

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
        Vector3 dir = (RoadManager.instance.roads[state.posIndex].position - state.transform.position).normalized;
        dir.y = 0;
        Vector3 moveVec = dir * state.speed * Time.fixedDeltaTime;
        state.rigid.MovePosition(state.rigid.position + moveVec);
        state.rigid.linearVelocity = Vector3.zero;

        if(dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            state.transform.rotation = Quaternion.Slerp(state.transform.rotation, targetRot, 5f * Time.fixedDeltaTime);
        }


        float dist = (RoadManager.instance.roads[state.posIndex].position - state.transform.position).sqrMagnitude;

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

        Vector3 dir = (RoadManager.instance.roads[state.posIndex].forward).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);

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
        state.paper.transform.parent = RoadManager.instance.paperPos;
        state.paper.transform.localPosition = Vector3.zero;
        state.paper.transform.localRotation = Quaternion.identity;
        state.paper.transform.localScale = Vector3.one;
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

        Vector3 dir = (RoadManager.instance.roads[state.posIndex].forward).normalized;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        state.rigid.isKinematic = true;
        state.transform.position = RoadManager.instance.sitPos.position;
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
    public Animator animator;
    public Rigidbody rigid;
    private StateMachine<MercenaryState, MercenaryController> stateMachine = new StateMachine<MercenaryState, MercenaryController>();
    public int posIndex = 0;
    public float speed;
    public bool isMove;
    public GameObject paper;
    public MercenaryState mercenaryState;

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

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void ChangeState(MercenaryState state)
    {
        stateMachine.ChangeState(state);
        mercenaryState = state;
    }
}
