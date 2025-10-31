using UnityEngine;

public enum BulletType
{
    Arrow, Magic
}

public class Arrow : MonoBehaviour
{
    private UnitController unit;
    private MonsterController target;
    [SerializeField] private float speed;
    private ParticleSystem effect;
    private bool isActive;
    [SerializeField] private BulletType bulletType;

    public void SetTarget( UnitController u, MonsterController t, ParticleSystem _effect)
    {
        unit = u;
        target = t;
        effect = _effect;
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
            float damage;

            if (bulletType == BulletType.Arrow)
            {
                effect.transform.position = monster.transform.position + Vector3.up * 2;
                damage = unit.atk;
            }
            else
            {
                effect.transform.position = monster.transform.position;
                damage = unit.atk * unit.skillDmg;
            }
            effect.Play();
            if (Random.value < unit.critRate)
            {
                other.GetComponent<IInteraction>().TakeHit(damage * unit.critDmg, TextType.Critical);
            }
            else
            {
                other.GetComponent<IInteraction>().TakeHit(damage, TextType.Normal);
            }

            if(bulletType == BulletType.Arrow)
            {
                unit.EnterArrow(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
