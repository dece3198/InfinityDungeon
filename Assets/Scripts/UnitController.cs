using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                state.canvas.eulerAngles = Vector3.zero;
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
        state.animator.SetTrigger("Attack");
        state.rigid.linearVelocity = Vector3.zero;
        if(state.isAD)
        {
            state.StartCoroutine(PlayBowAnimation(state));
        }
        state.StartCoroutine(AtkCo(state));
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

    private IEnumerator PlayBowAnimation(UnitController state)
    {
        state.Mp += 10;
        yield return new WaitForSeconds(0.5f);
        state.band.DOMove(state.pullPosition.position, 0.15f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.1f);
        state.band.DOMove(state.restPosition.position, 0.08f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.08f);
        state.ExitArrow();
    }

    private IEnumerator AtkCo(UnitController state)
    {
        yield return new WaitForSeconds(state.atkSpeed);
        if(state.unitState != UnitState.Skill)
        {
            state.ChangeState(UnitState.Idle);
        }
    }
}

public class UnitSkill : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
        switch(state.mercenary.mercenaryClass)
        {
            case MercenaryClass.SwordMan : state.StartCoroutine(SwordManSkill(state)); break;
        }
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

    private IEnumerator SwordManSkill(UnitController state)
    {
        state.animator.SetTrigger("Skill_0");
        state.skill[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        state.skill[1].gameObject.SetActive(false);
        state.skill[0].transform.position = state.skillPos.position;
        Vector3 rot = state.skill[0].transform.eulerAngles;
        rot.y = state.transform.eulerAngles.y;
        state.skill[0].transform.eulerAngles = rot;
        state.skill[0].GetComponent<Bullet>().BulletSetting(state.atk * state.skillDmg, state.critRate);
        state.skill[0].gameObject.SetActive(true);
        state.skill[0].Play();
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
            hpBar.value = Mathf.Clamp01(hp / maxHp);
        }
    }
    [SerializeField] private float mp;
    public float Mp
    {
        get { return mp; }
        set
        {
            mp = value;
            mpBar.value = Mathf.Clamp01(mp / maxMp);
            if(mp >= maxMp)
            {
                mp -= maxMp;
                ChangeState(UnitState.Skill);
            }
        }
    }
    public float maxMp;
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
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;
    public Transform canvas;
    public Transform skillPos;
    public ParticleSystem[] skill;
    [SerializeField] private GameObject arrowtPrefab;
    private Stack<GameObject> arrowStack = new Stack<GameObject> ();
    public Transform band;
    public Transform restPosition;
    public Transform pullPosition;

    private StateMachine<UnitState, UnitController> stateMachine = new StateMachine<UnitState, UnitController>();

    public Vector3 orignPos;

    public bool isMan;
    public bool isWait = true;
    public bool isSelect = false;
    public bool isAD = false;

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

    private void Start()
    {
        if(isAD)
        {
            for(int i = 0; i < 10; i++)
            {
                GameObject bullet = Instantiate(arrowtPrefab, band);
                arrowStack.Push(bullet);
            }
        }
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

    public void Attack()
    {
        viewDetector.FindAttackTarget();
        if(viewDetector.AttackTarget != null)
        {
            Mp += 10;
            if (Random.value < critRate)
            {
                viewDetector.AttackTarget.GetComponent<IInteraction>().TakeHit(atk * critDmg, TextType.Critical);
            }
            else
            {
                viewDetector.AttackTarget.GetComponent<IInteraction>().TakeHit(atk, TextType.Normal);
            }
            
        }
    }

    public void ExitArrow()
    {
        viewDetector.FindAttackTarget();
        if (viewDetector.AttackTarget != null)
        {
            GameObject arrow = arrowStack.Pop();
            arrow.transform.position = band.position;
            MonsterController targetCtrl = viewDetector.AttackTarget.GetComponent<MonsterController>();
            arrow.GetComponent<Arrow>().SetTarget(this, targetCtrl);
            arrow.SetActive(true);
            arrow.transform.SetParent(null);
        }
    }

    public void EnterArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        arrow.transform.parent = transform;
        arrowStack.Push(arrow);
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
