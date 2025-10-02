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

    public void FindTarget()
    {
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

    public void FindAttackTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRadius, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 findTarget = (targets[i].transform.position - transform.position).normalized;
            if (Vector3.Dot(transform.forward, findTarget) < Mathf.Cos(atkAngle * 0.5f * Mathf.Deg2Rad))
            {
                continue;
            }

            attackTarget = targets[i].gameObject;
            return;
        }

        attackTarget = null;
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
