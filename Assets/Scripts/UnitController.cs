using UnityEngine;

public enum UnitState
{
    Idle, Walk, Stun, Attack, Skill, Die
}

public class UnitIdle : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", false);
        state.rigid.linearVelocity = Vector3.zero;
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
    }

    public override void Update(UnitController state)
    {
        if (StageManager.instance.isStage && state.isWait)
        {
            state.viewDetector.FindTarget();
            if (state.viewDetector.Target != null)
            {
                state.ChangeState(UnitState.Walk);
            }
        }
    }
}

public class UnitWalk : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", true);
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
        if (state.viewDetector.Target == null)
        {
            state.ChangeState(UnitState.Idle);
        }
        else
        {
            Transform target = state.viewDetector.Target.transform;
            Vector3 dir = (target.position - state.transform.position).normalized;
            dir.y = 0f;
            Vector3 moveVec = dir * state.speed * Time.fixedDeltaTime;
            state.rigid.MovePosition(state.rigid.position + moveVec);
            state.rigid.linearVelocity = Vector3.zero;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                state.transform.rotation = Quaternion.Slerp(state.transform.rotation, targetRot, 0.2f);
            }
        }
    }

    public override void Update(UnitController state)
    {
        state.viewDetector.FindAttackTarget();
        if(state.viewDetector.AttackTarget != null)
        {
            state.ChangeState(UnitState.Attack);
        }
    }
}

public class UnitStun : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
    }

    public override void Update(UnitController state)
    {
    }
}

public class UnitAttack : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", false);
        state.rigid.linearVelocity = Vector3.zero;
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
    }

    public override void Update(UnitController state)
    {
    }
}

public class UnitSkill : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
    }

    public override void Update(UnitController state)
    {
    }
}

public class UnitDie : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
    }

    public override void Update(UnitController state)
    {
    }
}

public class UnitController : MonoBehaviour
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
        }
    }
    public float maxHp;
    public float atk;
    public float atkSpeed;
    public float def;
    public float critRate;
    public float critDmg;
    public float skillDmg;
    public float speed;
    public Mercenary mercenary;
    public Animator animator;
    public Rigidbody rigid;
    public ViewDetector viewDetector;
    public UnitState unitState;

    private StateMachine<UnitState, UnitController> stateMachine = new StateMachine<UnitState, UnitController>();

    public Vector3 orignPos;

    public bool isMan;
    public bool isWait = true;
    public bool isSelect = false;

    private void Awake()
    {
        maxHp = Hp;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        viewDetector = GetComponent<ViewDetector>();
        stateMachine.Reset(this);
        stateMachine.AddState(UnitState.Idle, new UnitIdle());
        stateMachine.AddState(UnitState.Walk, new UnitWalk());
        stateMachine.AddState(UnitState.Stun, new UnitStun());
        stateMachine.AddState(UnitState.Attack, new UnitAttack());
        stateMachine.AddState(UnitState.Skill, new UnitSkill());
        stateMachine.AddState(UnitState.Die, new UnitDie());
        ChangeState(UnitState.Idle);
    }

    private void OnEnable()
    {
        atk = mercenary.atk;
        atkSpeed = mercenary.atkSpeed;
        def = mercenary.def;
        Hp = mercenary.hp;
        maxHp = Hp;
        critRate = mercenary.criticalPercent;
        critDmg = mercenary.criticalDamage;
        skillDmg = mercenary.skillDamage;
        speed = mercenary.speed;
    }

    public void OnSelect()
    {
        //잡을때 효과
        isSelect = true;
        orignPos = transform.position;
    }
      
    public void FollowMouse(Vector3 targetPos)
    {
        rigid.linearVelocity = Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }

    public void OnRelease()
    {
        if(MouseController.instance.curSlot != null)
        {
            transform.position = MouseController.instance.curSlot.transform.position;
            if(MouseController.instance.curSlot.slotType == UnitSlotType.Waiting)
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
        isSelect = false;
    }

    public void Update()
    {
        stateMachine.Update();
    }

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void ChangeState(UnitState state)
    {
        stateMachine.ChangeState(state);
        unitState = state;
    }
}
