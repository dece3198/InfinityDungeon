using UnityEngine;

public class Arrow : MonoBehaviour
{
    private UnitController unit;
    private MonsterController target;
    [SerializeField] private float speed;
    private bool isActive;

    public void SetTarget( UnitController u, MonsterController t)
    {
        unit = u;
        target = t;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive || target == null) return;

        Vector3 dir = (target.center.position - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if(dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterController monster = other.GetComponent<MonsterController>();

        if (monster != null && monster == target)
        {
            if(Random.value < unit.critRate)
            {
                other.GetComponent<IInteraction>().TakeHit(unit.atk * unit.critDmg, TextType.Critical);
            }
            else
            {
                other.GetComponent<IInteraction>().TakeHit(unit.atk, TextType.Normal);
            }

            unit.EnterArrow(gameObject);
        }
    }
}
