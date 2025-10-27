using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewDetector : MonoBehaviour
{
    [SerializeField] private GameObject target;
    public GameObject Target { get { return target; } }
    [SerializeField] private GameObject attackTarget;
    public GameObject AttackTarget { get { return attackTarget; } }

    [SerializeField] private float radius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float angle;
    [SerializeField] private float atkAngle;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask sheldMask;
    private UnitController controller;

    private void Awake()
    {
        controller = GetComponent<UnitController>();
    }

    public void FindTarget()
    {
        if (Target != null)
        {
            // ����� ������ �����ϰ� Ȱ��ȭ�Ǿ� ������, �Ÿ� �ȿ� �ִٸ� ����
            float dist = Vector3.Distance(transform.position, Target.transform.position);
            if (Target.activeSelf)
            {
                return; // ���� Ÿ�� ��� ���
            }

            // Ÿ���� ������ų� �ʹ� �ָ� ����� �ʱ�ȭ
            attackTarget = null;
        }

        Collider[] targets = Physics.OverlapSphere(transform.position, radius, layerMask);
        float min = Mathf.Infinity;

        for(int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if(Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }
            
            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);

            Debug.DrawRay(transform.position, findTarget *  findTargetRange, Color.red);

            if(findTargetRange < min)
            {
                min = findTargetRange;
                target = targets[i].gameObject;
            }
        }

        if(targets.Length <= 0)
        {
            target = null;
        }
    }

    public void FindUnitTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, layerMask);
        float min = Mathf.Infinity;
        GameObject bestTarget = null;

        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].TryGetComponent(out UnitController unit))
                continue;

            if (!unit.isWait)
                continue;

            Vector3 findTarget = (unit.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
                continue;

            float findTargetRange = Vector3.Distance(transform.position, unit.transform.position);
            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            if (findTargetRange < min)
            {
                min = findTargetRange;
                bestTarget = unit.gameObject;
            }
        }

        target = bestTarget;
    }

    public void FindAttackTarget()
    {
        //���� Ÿ�� ���� ����
        if (attackTarget != null)
        {
            // ����� ������ �����ϰ� Ȱ��ȭ�Ǿ� ������, �Ÿ� �ȿ� �ִٸ� ����
            float dist = Vector3.Distance(transform.position, attackTarget.transform.position);
            if (attackTarget.activeSelf && dist <= attackRadius * 2f)
            {
                return; // ���� Ÿ�� ��� ���
            }

            // Ÿ���� ������ų� �ʹ� �ָ� ����� �ʱ�ȭ
            attackTarget = null;
        }

        //�� Ÿ�� Ž��
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRadius, layerMask);
        float min = Mathf.Infinity;
        GameObject bestTarget = null;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject == gameObject)
                continue;

            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(atkAngle * 0.5f * Mathf.Deg2Rad))
                continue;

            float dist = Vector3.Distance(transform.position, targets[i].transform.position);
            if (dist < min)
            {
                min = dist;
                bestTarget = targets[i].gameObject;
            }
        }

        attackTarget = bestTarget;
    }

    public List<UnitController> FindSheldTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, sheldMask);
        List<UnitController> allies = new List<UnitController>();

        foreach(var col in targets)
        {
            if(col.TryGetComponent(out UnitController unit))
            {
                if(unit.isWait && unit != controller)
                {
                    allies.Add(unit);
                }
            }
        }

        allies.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );

        return allies.Take(2).ToList();
    }

    public void FindHealTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, layerMask);
        float minDistance = Mathf.Infinity;
        float maxMissingHp = 0;
        GameObject bestTarget = null;

        foreach (var col in targets)
        {
            if (!col.TryGetComponent(out UnitController unit))
                continue;

            // �ڱ� �ڽ��� ����
            if (unit == controller)
                continue;

            // ��� ���� �ƴ�(isWait) ���ָ� �� ����
            if (!unit.isWait)
                continue;

            // �þ߰� üũ
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, dirToTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
                continue;

            // �켱 ���� �ʿ��� ���� ã��
            float missingHp = unit.maxHp - unit.Hp;
            if (missingHp > 0.01f) // ü���� �� �� ������ �ִٸ�
            {
                if (missingHp > maxMissingHp)
                {
                    maxMissingHp = missingHp;
                    bestTarget = unit.gameObject;
                }
            }
            // ���� ü���� ���� á���� �� ����� �Ʊ� �� �� �� ����
            else if (maxMissingHp == 0)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestTarget = unit.gameObject;
                }
            }
        }

        target = bestTarget;
    }

    public void AllFindHealHeal(float value)
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRadius * 1.5f, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }

            target = targets[i].gameObject;

            if (!target.TryGetComponent(out UnitController unit))
                continue;

            unit.Healing(value);
        }
        target = null;
    }

    public void FindFarTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, layerMask);
        float max = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }

            float findTargetRange = Vector3.Distance(transform.position, targets[i].transform.position);

            Debug.DrawRay(transform.position, findTarget * findTargetRange, Color.red);

            if (findTargetRange > max)
            {
                max = findTargetRange;
                target = targets[i].gameObject;
            }
        }

        if (targets.Length <= 0)
        {
            target = null;
        }
    }

    public void FindRangeAttack(float skillDmg, float critRate, float critDmg)
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRadius * 2, layerMask);

        for(int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].TryGetComponent(out IInteraction m))
                continue;

            if(Random.value < critRate)
            {
                m.TakeHit(skillDmg * critDmg, TextType.Critical);
            }
            else
            {
                m.TakeHit(skillDmg, TextType.Normal);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Vector3 lookDir = AngleToDir(transform.eulerAngles.y);
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + angle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - angle * 0.5f);

        Debug.DrawRay(transform.position, lookDir * radius, Color.green);
        Debug.DrawRay(transform.position, rightDir * radius, Color.green);
        Debug.DrawRay(transform.position, leftDir * radius, Color.green);
    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian));
    }
}
