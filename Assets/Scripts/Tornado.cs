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

                if (Random.value < unit.critRate)
                {
                    StartCoroutine(AttackCo(unit, TextType.Critical, interaction));
                }
                else
                {
                    StartCoroutine(AttackCo(unit, TextType.Normal, interaction));
                }
            }
        }
    }

    private IEnumerator AttackCo(UnitController unit, TextType textType, IInteraction interaction)
    {
        isAtk = false;
        interaction.TakeHit(unit.atk * unit.skillDmg, textType);
        yield return new WaitForSeconds(0.5f);
        isAtk = true;
    }

    private IEnumerator TornadoCo()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
