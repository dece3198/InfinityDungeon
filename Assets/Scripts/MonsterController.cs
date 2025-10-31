using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterIdle : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
        state.animator.SetBool("Move", false);
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
    }

    public override void Update(MonsterController state)
    {
        if(StageManager.instance.isStage)
        {
            state.viewDetector.FindUnitTarget();
            if (state.viewDetector.Target != null)
            {
                state.ChangeState(MonsterState.Walk);
            }
        }
    }
}

public class MonsterWalk : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
        state.animator.SetBool("Move", true);
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
        if (state.viewDetector.Target == null)
        {
            state.ChangeState(MonsterState.Idle);
        }
        else
        {
            state.canvas.eulerAngles = Vector3.zero;
            Transform target = state.viewDetector.Target.transform;
            Vector3 dir = (target.position - state.transform.position).normalized;
            dir.y = 0f;
            float distance = Vector3.Distance(target.position, state.transform.position);
            Vector3 moveVec = dir * state.speed * Time.fixedDeltaTime;
            if (state.viewDetector.attackRadius < distance)
            {
                state.rigid.MovePosition(state.rigid.position + moveVec);
            }
            state.rigid.linearVelocity = Vector3.zero;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                state.transform.rotation = Quaternion.Slerp(state.transform.rotation, targetRot, 5f);
            }

        }
    }

    public override void Update(MonsterController state)
    {
        if(state.isProvoke)
        {
            state.viewDetector.FindAttackTarget();
            if (state.viewDetector.AttackTarget != null)
            {
                state.ChangeState(MonsterState.Attack);
            }
        }
    }
}

public class MonsterStun : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
        state.animator.SetBool("Move", false);
        state.animator.SetBool("Stun", true);
        state.StartCoroutine(StunCo(state));
        state.stunEffect.SetActive(true);
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
    }

    public override void Update(MonsterController state)
    {
    }

    private IEnumerator StunCo(MonsterController state)
    {
        yield return new WaitForSeconds(state.stun);
        state.animator.SetBool("Stun", false);
        state.stunEffect.SetActive(false);
        if (state.monsterState == MonsterState.Stun)
        {
            state.ChangeState(MonsterState.Idle);
        }
    }
}

public class MonsterAttack : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
        state.animator.SetBool("Move", false);
        state.animator.SetTrigger("Attack");
        state.rigid.linearVelocity = Vector3.zero;
        state.StartCoroutine(AttackCo(state));
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
        state.canvas.eulerAngles = Vector3.zero;
    }

    public override void Update(MonsterController state)
    {
    }

    private IEnumerator AttackCo(MonsterController state)
    {
        yield return new WaitForSeconds(state.atkSpeed);
        if(state.monsterState == MonsterState.Attack)
        {
            state.ChangeState(MonsterState.Idle);
        }
    }
}

public class MonsterSkill : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
    }

    public override void Update(MonsterController state)
    {
    }
}

public class MonsterDie : BaseState<MonsterController>
{
    public override void Enter(MonsterController state)
    {
    }

    public override void Exit(MonsterController state)
    {
    }

    public override void FixedUpdate(MonsterController state)
    {
    }

    public override void Update(MonsterController state)
    {
    }
}

public class MonsterController : Monster, IInteraction
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set 
        { 
            hp = value;
            hpBar.value = Mathf.Clamp01(hp / maxHp);
        } 
    }

    public float maxHp;
    public float speed;
    public float atk;
    public float atkSpeed;
    public float stun;
    public GameObject stunEffect;
    [SerializeField] SkinnedMeshRenderer[] renderers;
    [SerializeField] private TextManager textManager;
    [SerializeField] private Slider hpBar;
    public Transform canvas;
    public MonsterState monsterState;
    public Animator animator;
    public Rigidbody rigid;
    public ViewDetector viewDetector;
    public Transform center;
    private StateMachine<MonsterState, MonsterController> stateMachine = new StateMachine<MonsterState, MonsterController>();

    public bool isProvoke = true;

    private void Awake()
    {
        maxHp = hp;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        viewDetector = GetComponent<ViewDetector>();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Idle, new MonsterIdle());
        stateMachine.AddState(MonsterState.Walk, new MonsterWalk());
        stateMachine.AddState(MonsterState.Stun, new MonsterStun());
        stateMachine.AddState(MonsterState.Attack, new MonsterAttack());
        stateMachine.AddState(MonsterState.Skill, new MonsterSkill());
        stateMachine.AddState(MonsterState.Die, new MonsterDie());
        ChangeState(MonsterState.Idle);
    }

    private void OnEnable()
    {
        Hp = maxHp;
        ChangeState(MonsterState.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void ChangeState(MonsterState state)
    {
        stateMachine.ChangeState(state);
        monsterState = state;
    }

    public void Attack()
    {
        viewDetector.FindAttackTarget();
        if (viewDetector.AttackTarget != null)
        {
            viewDetector.AttackTarget.GetComponent<UnitController>().TakeHit(atk);
        }
    }

    public void ProvokeStart()
    {
        StartCoroutine(ProvokeCo());
    }

    public void TakeHit(float damage, TextType textType)
    {
        Hp -= damage;
        textManager.ShowDamageText(damage, textType);
        StartCoroutine(HitCo());
    }

    private IEnumerator ProvokeCo()
    {
        isProvoke = false;
        yield return new WaitForSeconds(3f);
        isProvoke = true;
    }

    private IEnumerator HitCo()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.white;
        }
    }
}
