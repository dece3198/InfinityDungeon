using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject subCam;
    [SerializeField] private Animator[] StartAni;
    [SerializeField] private RectTransform fadeInOut;

    private void Awake()
    {
        fadeInOut.sizeDelta = new Vector2(5000, 5000);
        fadeInOut.gameObject.SetActive(false);
    }

    public void StartButton()
    {
        if(GameManager.instance.isStart)
        {
            StartCoroutine(StartCo());
        }
    }

    private IEnumerator StartCo()
    {
        GameManager.instance.isStart = false;
        StartAni[0].SetTrigger("Open");
        StartAni[1].SetTrigger("Open");
        yield return new WaitForSeconds(2f);
        StartAni[2].SetTrigger("Open");
        yield return new WaitForSeconds(1f);
        subCam.SetActive(true);
        yield return new WaitForSeconds(1f);
        fadeInOut.gameObject.SetActive(true);
        fadeInOut.DOSizeDelta(Vector2.zero, 1f);
        yield return new WaitForSeconds(1.5f);
        AsyncOperation op = SceneManager.LoadSceneAsync("LobbyScene");
        op.allowSceneActivation = false;
        while(op.progress < 0.9f)
        {
            yield return null;
        }
        op.allowSceneActivation = true;
        GameManager.instance.isStart = true;
    }
}
