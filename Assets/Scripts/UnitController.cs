using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitState
{
    Idle, Walk, Stun, Attack, Skill, Die
}

public abstract class SkillBehaviour
{
    public abstract IEnumerator Skill(UnitController unit);
}

public class SwordsWomanSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        unit.skill[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        unit.skill[2].gameObject.SetActive(false);
        unit.skill[1].transform.position = unit.skillPos.position;
        unit.skill[1].transform.rotation = Quaternion.Euler(0, unit.transform.eulerAngles.y, 90);
        unit.skill[1].GetComponent<Bullet>().BulletSetting(unit.atk * unit.skillDmg, unit.critRate, unit.critDmg);
        unit.skill[1].gameObject.SetActive(true);
        unit.skill[1].Play();
        if(unit.viewDetector.AttackTarget != null)
        {
            unit.skill[3].transform.position = unit.viewDetector.AttackTarget.transform.position;
            unit.skill[3].Play();
        }
    }
}

public class ShielderSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");

        List<UnitController> allies = unit.viewDetector.FindSheldTarget();

        unit.StartCoroutine(ApplyShield(unit, unit, unit.skill[3]));

        if (allies == null || allies.Count == 0)
            yield break;

        for (int i = 0; i < allies.Count; i++)
        {
            unit.StartCoroutine(ApplyShield(unit, allies[i], unit.skill[i + 1]));
        }
    }

    private IEnumerator ApplyShield(UnitController state, UnitController target, ParticleSystem skill)
    {
        yield return new WaitForSeconds(0.1f);
        skill.transform.position = state.skillPos.position;
        skill.gameObject.SetActive(true);
        skill.Play();
        Vector3 upPos = state.transform.position + Vector3.up * 5f;
        skill.transform.DOMove(upPos, 0.3f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        skill.transform.DOMove(target.transform.position + Vector3.up * 2f, 0.4f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.4f);
        target.shieldObj = skill.gameObject;
        target.ShieldOn(state.atk * state.skillDmg);
    }
}

public class ArcherSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");

        yield return new WaitForSeconds(1.25f);
        unit.skill[2].gameObject.SetActive(true);
        unit.skill[2].Play();
        unit.band.DOMove(unit.pullPosition.position, 0.15f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.42f);
        unit.band.DOMove(unit.restPosition.position, 0.08f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.08f);
        unit.skill[1].transform.position = unit.skillPos.position;
        unit.viewDetector.FindAttackTarget();
        if (unit.viewDetector.AttackTarget != null)
        {
            unit.skill[1].GetComponent<Tornado>().SetTarget(unit, unit.viewDetector.AttackTarget.GetComponent<MonsterController>());
        }
        unit.skill[2].gameObject.SetActive(false);
        unit.skill[1].gameObject.SetActive(true);
        unit.skill[1].Play();
    }
}

public class HealerSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(0.5f);
        unit.skill[1].Play();
        unit.viewDetector.AllFindHealHeal(unit.atk * unit.skillDmg);
    }
}

public class AssassinSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(0.8f);
        unit.gameObject.layer = 0;
        unit.band.gameObject.SetActive(false);
        unit.pullPosition.gameObject.SetActive(false);
        unit.restPosition.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        unit.skill[1].Play();
        yield return new WaitForSeconds(1f);
        unit.viewDetector.FindFarTarget();
        if (unit.viewDetector.Target != null)
        {
            Transform t = unit.viewDetector.Target.transform;
            Vector3 behind = t.position - t.forward * 2f; // 또는 t.position + (-t.forward * 2f)
            unit.transform.position = behind;
        }
        yield return new WaitForSeconds(0.5f);
        unit.skill[2].Play();
        Transform target = unit.viewDetector.Target.transform;
        Vector3 dir = (target.position - unit.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        unit.transform.rotation = Quaternion.Slerp(unit.transform.rotation, targetRot, 5f);
        yield return new WaitForSeconds(0.5f);
        unit.band.gameObject.SetActive(true);
        unit.pullPosition.gameObject.SetActive(true);
        unit.restPosition.gameObject.SetActive(true);
        unit.animator.SetTrigger("Skill_1");
        yield return new WaitForSeconds(0.5f);
        if(unit.viewDetector.Target != null && unit.viewDetector.Target.TryGetComponent(out IInteraction m))
        {
            float skillD = unit.atk * unit.skillDmg;
            if (Random.value < unit.critRate)
            {
                m.TakeHit(skillD * unit.critDmg, TextType.Critical);
            }
            else
            {
                m.TakeHit(skillD, TextType.Normal);
            }
        }
        unit.skill[3].Play();
        yield return new WaitForSeconds(0.5f);
        unit.gameObject.layer = 7;
        unit.ChangeState(UnitState.Idle);
    }
}

public class MaulerSkill : SkillBehaviour
{
    private Vector3 orignScale;
    public override IEnumerator Skill(UnitController unit)
    {
        unit.atk = unit.atk * 2;
        unit.Hp += unit.maxHp;
        unit.maxHp = unit.maxHp * 2;
        orignScale = unit.transform.localScale;
        unit.transform.DOScale(orignScale * 2, 1.5f).SetEase(Ease.OutBack);
        unit.ChangeState(UnitState.Idle);
        yield return new WaitForSeconds(10f);
        unit.transform.DOScale(orignScale, 1f).SetEase(Ease.Linear);
        unit.atk /= 2;
        unit.maxHp /= 2;
        unit.Hp -= unit.maxHp;
    }
}

public class LancerSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(0.1f);
        unit.skill[1].gameObject.SetActive(true);
        float damage = unit.atk * unit.skillDmg;
        for(int i = 0; i < 8; i++)
        {
            unit.viewDetector.FindRangeAttack(damage, unit.critRate, unit.critDmg);
            yield return new WaitForSeconds(0.1733f);
        }
        unit.skill[1].gameObject.SetActive(false);
    }
}

public class BerserkerSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(0.75f);
        unit.skill[3].gameObject.SetActive(true);
        unit.skill[1].Play();
        yield return new WaitForSeconds(1f);
        unit.skill[2].Play();
        float skillD = unit.atk * unit.skillDmg;
        unit.viewDetector.FindRangeAttack(skillD, unit.critRate, unit.critDmg);
        unit.skill[3].gameObject.SetActive(false);
    }
}

public class HunterSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_1");
        yield return new WaitForSeconds(0.5f);
        unit.band.DOMove(unit.pullPosition.position, 0.15f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.5f);
        unit.band.DOMove(unit.restPosition.position, 0.08f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.07f);
        unit.skill[1].Play();
        yield return new WaitForSeconds(0.5f);
        unit.viewDetector.FindAttackTarget();
        if(unit.viewDetector.AttackTarget != null)
        {
            unit.skillPos.position = unit.viewDetector.AttackTarget.transform.position;
            unit.skill[2].Play();
            yield return new WaitForSeconds(1.5f);
            ViewDetector skill = unit.skillPos.GetComponent<ViewDetector>();
            float skillD = unit.atk * unit.skillDmg;
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.15f);
                skill.FindRangeAttack(skillD, unit.critRate, unit.critDmg);
            }
        }
    }
}

public class WarriorSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        unit.restPosition.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        unit.pullPosition.gameObject.SetActive(true);
        unit.pullPosition.DOScale(unit.pullPosition.localScale * 2,1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.25f);
        unit.skill[1].transform.position = unit.viewDetector.AttackTarget.transform.position;
        unit.skill[1].Play();
        if(unit.skill[1].TryGetComponent(out ViewDetector v))
        {
            float skillD = unit.atk * unit.skillDmg;
            v.FindRangeAttack(skillD, unit.critRate, unit.critDmg);
        }
        yield return new WaitForSeconds(1f);
        unit.pullPosition.DOScale(unit.pullPosition.localScale / 2, 1f).SetEase(Ease.OutBack);
        unit.pullPosition.gameObject.SetActive(false);
        unit.restPosition.gameObject.SetActive(true);
    }
}

public class ThrowerSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(1f);
        unit.skill[1].Play();
        if(unit.viewDetector.AttackTarget != null)
        {
            unit.skill[1].transform.DOMove(unit.viewDetector.AttackTarget.transform.position + Vector3.up * 20f, 0.5f).SetEase(Ease.Linear);
        }
        yield return new WaitForSeconds(1f);
        unit.viewDetector.FindAttackTarget();
        if (unit.viewDetector.AttackTarget != null)
        {
            unit.skillPos.transform.position = unit.viewDetector.AttackTarget.transform.position;
            unit.skill[2].Play();
            yield return new WaitForSeconds(0.5f);
            ViewDetector skill = unit.skillPos.GetComponent<ViewDetector>();
            float skillD = unit.atk * unit.skillDmg;
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.15f);
                skill.FindRangeAttack(skillD, unit.critRate, unit.critDmg);
            }
        }
    }
}

public class TankerSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(0.5f);
        unit.viewDetector.ProvokeStart();
        unit.skill[1].Play();
        unit.skill[2].gameObject.SetActive(true);
        float orignDef = unit.def;
        unit.def *= 2;
        yield return new WaitForSeconds(10);
        unit.def = orignDef;
        unit.skill[2].gameObject.SetActive(false);
    }
}

public class FocusWizardSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        unit.skill[3].gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        unit.skillPos.position = unit.viewDetector.AttackTarget.transform.position + Vector3.up * 40f;
        if(unit.skillPos.TryGetComponent(out Arrow s) && unit.viewDetector.AttackTarget.TryGetComponent(out MonsterController m))
        {
            s.SetTarget(unit, m, unit.skill[2]);
            s.gameObject.SetActive(true);
        }
        unit.skill[3].gameObject.SetActive(false);
    }
}

public class AreaWizardSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        unit.skill[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        unit.skill[3].transform.position = unit.viewDetector.AttackTarget.transform.position;
        unit.skill[3].Play();
        if (unit.skill[3].TryGetComponent(out ViewDetector v))
        {
            v.FindStunTarget(3.5f);
        }
        unit.skill[2].gameObject.SetActive(false);
    }
}

public class TankerBotSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        unit.skill[1].Play();
        float tempDef = unit.def;
        unit.def *= 2;
        yield return new WaitForSeconds(10f);
        unit.def = tempDef;
    }
}

public class SubTankerBotSkill : SkillBehaviour
{
    public override IEnumerator Skill(UnitController unit)
    {
        unit.animator.SetTrigger("Skill_0");
        yield return new WaitForSeconds(1f);
        unit.skill[1].Play();
        float skillDmg = unit.atk * unit.skillDmg;
        unit.viewDetector.FindRangeAttack(skillDmg, unit.critRate,unit.critDmg);
    }
}

public class UnitIdle : BaseState<UnitController>
{
    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", false);
        if(!state.rigid.isKinematic)
        {
            state.rigid.linearVelocity = Vector3.zero;
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
        if (StageManager.instance.isStage && !state.isWait)
        {
            if(state.mercenary.mercenaryClass == MercenaryClass.Healer)
            {
                state.viewDetector.FindHealTarget();
            }
            else
            {
                state.viewDetector.FindTarget();
            }

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
        state.rigid.isKinematic = false;
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
            state.textManager.transform.eulerAngles = Vector3.zero;
            Transform target = state.viewDetector.Target.transform;
            Vector3 dir = (target.position - state.transform.position).normalized;
            dir.y = 0f;
            Vector3 moveVec = dir * state.speed * Time.fixedDeltaTime;
            if (state.viewDetector.AttackTarget == null)
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

    public override void Update(UnitController state)
    {
        state.viewDetector.FindAttackTarget();
        if(state.viewDetector.AttackTarget != null && state.viewDetector.AttackTarget != state.gameObject)
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
    private bool isAtk = false;
    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", false);
        state.rigid.linearVelocity = Vector3.zero;

        if(state.isAtk)
        {
            if(state.mercenary.mercenaryClass != MercenaryClass.Warrior)
            {
                state.animator.SetTrigger("Attack");
            }
            
            state.StartCoroutine(AtkCo(state));
            switch (state.mercenary.mercenaryClass)
            {
                case MercenaryClass.Archer: state.StartCoroutine(PlayBowAnimation(state)); break;
                case MercenaryClass.Healer: state.StartCoroutine(Healing(state)); break;
                case MercenaryClass.Hunter: state.StartCoroutine(PlayBowAnimation(state)); break;
                case MercenaryClass.Warrior: state.StartCoroutine(WarriorAtkCo(state)); break;
                case MercenaryClass.Thrower: state.StartCoroutine(PlayBowAnimation(state)); break;
                case MercenaryClass.FocusWizard: state.StartCoroutine(WizardAttackCo(state)); break;
                case MercenaryClass.AreaWizard: state.StartCoroutine(WizardAttackCo(state)); break;
            }
        }
        else
        {
            state.ChangeState(UnitState.Idle);
        }
    }

    public override void Exit(UnitController state)
    {
    }

    public override void FixedUpdate(UnitController state)
    {
        state.textManager.transform.eulerAngles = Vector3.zero;
    }

    public override void Update(UnitController state)
    {
    }

    private IEnumerator PlayBowAnimation(UnitController state)
    {
        yield return new WaitForSeconds(0.49f);
        state.Mp += 10;
        state.ExitArrow(state.skill[0]);
    }

    private IEnumerator WizardAttackCo(UnitController state)
    {
        yield return new WaitForSeconds(0.5f);
        state.skill[1].Play();
        state.Mp += 10;
        state.ExitArrow(state.skill[0]);
    }

    private IEnumerator Healing(UnitController state)
    {
        yield return new WaitForSeconds(1f);
        state.Mp += 10;
        state.skill[0].transform.parent = state.viewDetector.Target.transform;
        state.skill[0].transform.localPosition = Vector3.zero;
        state.skill[0].Play();
        if (state.viewDetector.Target != null)
        {
            state.viewDetector.Target.GetComponent<UnitController>().Healing(state.atk);
        }
    }

    private IEnumerator AtkCo(UnitController state)
    {
        state.isAtk = false;
        yield return new WaitForSeconds(state.atkSpeed);
        if(state.unitState == UnitState.Attack)
        {
            state.ChangeState(UnitState.Idle);
        }
        state.isAtk = true;
    }

    private IEnumerator WarriorAtkCo(UnitController state)
    {
        isAtk = !isAtk;

        float orignAtk = state.atk;

        if (isAtk)
        {
            state.atk -= state.atk * 0.3f;
            state.animator.SetTrigger("Attack");
        }
        else
        {
            state.animator.SetTrigger("Attack2");
        }
        yield return new WaitForSeconds(state.atkSpeed);
        state.atk = orignAtk;
    }
}

public class UnitSkill : BaseState<UnitController>
{

    public override void Enter(UnitController state)
    {
        state.animator.SetBool("Move", false);
        state.UseSKill();
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
            if(mp >= maxMp)
            {
                mp -= maxMp;
                ChangeState(UnitState.Skill);
            }
            mpBar.value = Mathf.Clamp01(mp / maxMp);
        }
    }
    [SerializeField] private float shield;
    public float Shield
    {
        get { return shield; }
        set
        {
            shield = value;
            if(shield <= 0 && shieldObj != null)
            {
                shieldObj.SetActive(false);
            }
            shieldBar.value = Mathf.Clamp01(shield / maxShield);
        }
    }

    public float maxShield;
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
    [SerializeField] private Slider shieldBar;
    public Transform skillPos;
    public ParticleSystem[] skill;
    public ParticleSystem upEffect;
    [SerializeField] private GameObject arrowtPrefab;
    private Stack<GameObject> arrowStack = new Stack<GameObject> ();
    public Transform band;
    public Transform restPosition;
    public Transform pullPosition;
    public TextManager textManager;
    [SerializeField] private Transform center;
    private StateMachine<UnitState, UnitController> stateMachine = new StateMachine<UnitState, UnitController>();
    public GameObject shieldObj;
    public UnitSlot unitSlot;
    public UnitSlot tempSlot;
    private SkillBehaviour skillBehaviour;

    private List<Transform> icons = new List<Transform> ();
    [SerializeField] private Transform atkIcon;
    [SerializeField] private Transform defIcon;
    [SerializeField] private Transform hpIcon;
    [SerializeField] private ParticleSystem buffEffect;

    private Vector3 orignPos;
    public bool isMan;
    public bool isWait = true;
    public bool isSelect = false;
    public bool isAD = false;
    public bool isAtk = true;
    [SerializeField] private bool isBot = false;

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

        switch (mercenary.mercenaryClass)
        {
            case MercenaryClass.SwordsWoman: skillBehaviour = new SwordsWomanSkill(); break;
            case MercenaryClass.Shielder: skillBehaviour = new ShielderSkill(); break;
            case MercenaryClass.Archer: skillBehaviour = new ArcherSkill(); break;
            case MercenaryClass.Healer: skillBehaviour = new HealerSkill(); break;
            case MercenaryClass.Assassin: skillBehaviour = new AssassinSkill(); break;
            case MercenaryClass.Mauler: skillBehaviour = new MaulerSkill(); break;
            case MercenaryClass.Lancer: skillBehaviour = new LancerSkill(); break;
            case MercenaryClass.Berserker: skillBehaviour = new BerserkerSkill(); break;
            case MercenaryClass.Hunter: skillBehaviour = new HunterSkill(); break;
            case MercenaryClass.Warrior: skillBehaviour = new WarriorSkill(); break;
            case MercenaryClass.Thrower: skillBehaviour = new ThrowerSkill(); break;
            case MercenaryClass.Tanker: skillBehaviour = new TankerSkill(); break;
            case MercenaryClass.FocusWizard: skillBehaviour = new FocusWizardSkill(); break;
            case MercenaryClass.AreaWizard: skillBehaviour = new AreaWizardSkill(); break;
            case MercenaryClass.TankerBot: skillBehaviour = new TankerBotSkill(); break;
            case MercenaryClass.SubTankerBot: skillBehaviour = new SubTankerBotSkill(); break;
        }
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
        maxShield = 1;
        Shield = 0;
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

    public void Update()
    {
        stateMachine.Update();
    }

    public void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void AddStats()
    {
        atk = mercenary.atk;
        atkSpeed = mercenary.atkSpeed;
        def = mercenary.def;
        Hp = mercenary.hp;
        maxHp = Hp;
    }

    public void Buff(UseCard card)
    {
        switch (card.useType)
        {
            case UseType.UnitAtk:
                atk += atk * card.level * 0.01f;
                if (!icons.Contains(atkIcon))
                {
                    atkIcon.gameObject.SetActive(true);
                    icons.Add(atkIcon);
                }
                break;

            case UseType.UnitDef:
                def += def * card.level * 0.01f;
                if (!icons.Contains(defIcon))
                {
                    defIcon.gameObject.SetActive(true);
                    icons.Add(defIcon);
                }
                break;

            case UseType.UnitHp:
                Hp += Hp * card.level * 0.01f;
                if (!icons.Contains(hpIcon))
                {
                    hpIcon.gameObject.SetActive(true);
                    icons.Add(hpIcon);
                }
                break;
        }

        buffEffect.gameObject.SetActive(true);
        RefreshIconPositions();
    }

    private void RefreshIconPositions()
    {
        int count = icons.Count;
        if (count == 0) return;

        float startX = -(0.5f * (count - 1) * 0.5f);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = icons[i].localPosition;
            pos.x = startX + (0.5f * i);
            pos.y = 2.75f;
            icons[i].localPosition = pos;
        }
    }

    public void ResetUnit()
    {
        atkIcon.gameObject.SetActive(false);
        defIcon.gameObject.SetActive(false);
        hpIcon.gameObject.SetActive(false);
        buffEffect.gameObject.SetActive(false);
    }

    public void TakeHit(float damage)
    {
        if(Shield > 0)
        {
            if (Shield < damage)
            {
                Hp -= (damage - Shield);
                Shield = 0;
            }
            else
            {
                Shield -= damage;
            }
        }
        else
        {
            Hp -= damage;
        }
    }

    public void UseSKill()
    {
        StartCoroutine(skillBehaviour.Skill(this));
    }

    public void ShieldOn(float value)
    {
        StartCoroutine(ShieldCo(value));
    }

    public void Healing(float value)
    {
        textManager.ShowDamageText(value, TextType.Normal);
        float maxHeal = Hp + value;
        if(maxHeal > maxHp)
        {
            Hp = maxHp;
        }
        else
        {
            Hp += value;
        }
    }

    public void OnSelect()
    {
        rigid.isKinematic = false;
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
        rigid.isKinematic = true;

        UnitController other = unitSlot.controller;

        bool isLevel = DungeonManager.instance.LevelIndex >= DungeonManager.instance.Level;

        bool isWaitingSlot = unitSlot.slotType == UnitSlotType.Waiting;

        if (isLevel && !isWaitingSlot && other == null)
        {
            transform.position = orignPos;
            isSelect = false;
            if (unitSlot != null)
                unitSlot.check.SetActive(false);
            return;
        }

        if (other != null)
        {
            UnitSlot targetSlot = unitSlot;
            bool tempWait = other.isWait;

            other.transform.position = orignPos;
            transform.position = targetSlot.transform.position; 

            other.isWait = isWait;
            isWait = tempWait;

            tempSlot.controller = other;
            targetSlot.controller = this;

            other.tempSlot = tempSlot;
            tempSlot = unitSlot;

            if(isBot && other != this)
            {
                DungeonManager.instance.LevelIndex--;
            }
            
            if(other.isBot && other != this)
            {
                DungeonManager.instance.LevelIndex++;
            }
        }
        else
        {
            transform.position = unitSlot.transform.position;
            unitSlot.controller = this;
            
            if(tempSlot != null)
            {
                tempSlot.controller = null;
            }

            tempSlot = unitSlot;

            if (unitSlot.slotType == UnitSlotType.Waiting)
            {
                gameObject.layer = 0;
                if (!isWait)
                {
                    isWait = true;
                    if(!isBot)
                    {
                        DungeonManager.instance.LevelIndex--;
                    }
                }
            }
            else
            {
                gameObject.layer = 7;
                if (isWait)
                {
                    isWait = false;
                    if(!isBot)
                    {
                        DungeonManager.instance.LevelIndex++;
                    }
                }
            }
        }

        isSelect = false;
        if(unitSlot != null)
        {
            unitSlot.check.SetActive(false);
        }
    }

    public void Attack()
    {
        viewDetector.FindAttackTarget();
        if(viewDetector.AttackTarget != null)
        {
            Mp += 10;
            skill[0].transform.position = viewDetector.AttackTarget.transform.position + Vector3.up * 2;
            skill[0].Play();
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

    public void ExitArrow(ParticleSystem effect)
    {
        viewDetector.FindAttackTarget();
        if (viewDetector.AttackTarget != null)
        {
            GameObject arrow = arrowStack.Pop();
            arrow.transform.position = band.position;
            MonsterController targetCtrl = viewDetector.AttackTarget.GetComponent<MonsterController>();
            arrow.GetComponent<Arrow>().SetTarget(this, targetCtrl, effect);
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

    public void ChangeState(UnitState state)
    {
        stateMachine.ChangeState(state);
        unitState = state;
    }

    private IEnumerator ShieldCo(float value)
    {
        Shield = value;
        maxShield = Shield;

        shieldObj.transform.parent = transform;
        shieldObj.transform.position = center.position;
        yield return new WaitForSeconds(5f);
        shieldObj.gameObject.SetActive(false);
        Shield = 0;
    }
}
