using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private RectTransform fadeInOut;
    private Vector2 orignPos;


    private void Start()
    {
        orignPos = new Vector2(5000, 5000);
        fadeInOut.sizeDelta = Vector2.zero;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        fadeInOut.gameObject.SetActive(true);
        fadeInOut.DOSizeDelta(orignPos, 1f);
        yield return new WaitForSeconds(1f);
    }
}
