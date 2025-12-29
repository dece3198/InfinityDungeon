using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    private UnitController unit;
    [SerializeField] private float speed;
    private ViewDetector viewDetector;
    private bool isAtk;

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
    }

    public void SetTarget(UnitController u)
    {
        unit = u;
        isAtk = true;
    }

    private void OnEnable()
    {
        StartCoroutine(TornadoCo());
    }

    private void Update()
    {
        viewDetector.FindTarget();

        if(viewDetector.Target != null)
        {
            Vector3 dir = (viewDetector.Target.transform.position - transform.position).normalized;

            transform.position += dir * speed * Time.deltaTime;
        }
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
