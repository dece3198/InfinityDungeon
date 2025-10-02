using System.Collections;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private void OnEnable()
    {
        StartCoroutine(TextCo());
    }

    private IEnumerator TextCo()
    {
        float time = 1.5f;

        while(time > 0)
        {
            time -= Time.deltaTime;
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
            yield return null;
        }
    }

}
