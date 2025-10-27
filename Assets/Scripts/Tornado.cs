using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    private UnitController unit;
    private MonsterController target;
    [SerializeField] private float speed;
    private bool isActive;
    private bool isAtk;

    public void SetTarget(UnitController u, MonsterController t)
    {
        unit = u;
        target = t;
        isActive = true;
        isAtk = true;
    }

    private void OnEnable()
    {
        StartCoroutine(TornadoCo());
    }

    private void Update()
    {
        if (!isActive || target == null) return;

        Vector3 dir = (target.transform.position - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if(isAtk)
        {
            MonsterController monster = other.GetComponent<MonsterController>();

            if(monster != null)
            {
                IInteraction interaction  = other.GetComponent<IInteraction>();
                float skill = unit.atk * unit.skillDmg;
                if (Random.value < unit.critRate)
                {
                    StartCoroutine(AttackCo(skill * unit.critDmg, TextType.Critical, interaction));
                }
                else
                {
                    StartCoroutine(AttackCo(skill, TextType.Normal, interaction));
                }
            }
        }
    }

    private IEnumerator AttackCo(float damage, TextType textType, IInteraction interaction)
    {
        isAtk = false;
        interaction.TakeHit(damage, textType);
        yield return new WaitForSeconds(0.5f);
        isAtk = true;
    }

    private IEnumerator TornadoCo()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
