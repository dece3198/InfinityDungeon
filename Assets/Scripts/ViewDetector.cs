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
            // 대상이 여전히 존재하고 활성화되어 있으며, 거리 안에 있다면 유지
            float dist = Vector3.Distance(transform.position, Target.transform.position);
            if (Target.activeSelf)
            {
                return; // 기존 타겟 계속 사용
            }

            // 타겟이 사라지거나 너무 멀리 벗어나면 초기화
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
        //기존 타겟 유지 조건
        if (attackTarget != null)
        {
            // 대상이 여전히 존재하고 활성화되어 있으며, 거리 안에 있다면 유지
            float dist = Vector3.Distance(transform.position, attackTarget.transform.position);
            if (attackTarget.activeSelf && dist <= attackRadius * 2f)
            {
                return; // 기존 타겟 계속 사용
            }

            // 타겟이 사라지거나 너무 멀리 벗어나면 초기화
            attackTarget = null;
        }

        //새 타겟 탐색
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

            // 자기 자신은 제외
            if (unit == controller)
                continue;

            // 대기 중이 아닌(isWait) 유닛만 힐 가능
            if (!unit.isWait)
                continue;

            // 시야각 체크
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, dirToTarget) < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
                continue;

            // 우선 힐이 필요한 유닛 찾기
            float missingHp = unit.maxHp - unit.Hp;
            if (missingHp > 0.01f) // 체력이 덜 찬 유닛이 있다면
            {
                if (missingHp > maxMissingHp)
                {
                    maxMissingHp = missingHp;
                    bestTarget = unit.gameObject;
                }
            }
            // 전부 체력이 가득 찼으면 → 가까운 아군 중 한 명 선택
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
