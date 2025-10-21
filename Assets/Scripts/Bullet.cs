using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    public float critRate;
    public float damage;

    private void OnEnable()
    {
        StartCoroutine(StartCo());
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MonsterController>() != null)
        {
            if(Random.value < critRate)
            {
                other.GetComponent<IInteraction>().TakeHit(damage * 2, TextType.Critical);
            }
            else
            {
                other.GetComponent<IInteraction>().TakeHit(damage, TextType.Normal);
            }
        }
    }

    public void BulletSetting(float d, float P)
    {
        damage = d;
        critRate = P;
    }

    private IEnumerator StartCo()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
