using UnityEngine;

public class MonsterArrow : MonoBehaviour
{
    private MonsterController monster;
    private UnitController target;
    [SerializeField] private float speed;
    private ParticleSystem effect;
    private bool isActive;
    [SerializeField] private BulletType bulletType;

    public void SetTarget(MonsterController m, UnitController t, ParticleSystem _effect)
    {
        monster = m;
        target = t;
        effect = _effect;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive || target == null) return;

        Vector3 dir = (target.center.position - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitController unit = other.GetComponent<UnitController>();
        if (unit != null && unit == target)
        {
            float damage;

            if (bulletType == BulletType.Arrow)
            {
                effect.transform.position = unit.transform.position + Vector3.up * 2;
                damage = monster.atk;
            }
            else
            {
                effect.transform.position = unit.transform.position;
                damage = monster.atk * monster.skillDmg;
            }

            effect.Play();

            if (Random.value < monster.critRate)
            {
                other.GetComponent<IInteraction>().TakeHit(damage * monster.critDmg, TextType.Critical);
            }
            else
            {
                other.GetComponent<IInteraction>().TakeHit(damage, TextType.Normal);
            }

            if (bulletType == BulletType.Arrow)
            {
                monster.EnterArrow(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
