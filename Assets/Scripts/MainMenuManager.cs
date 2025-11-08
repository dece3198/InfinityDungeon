using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject subCam;
    [SerializeField] private Animator[] StartAni;
    [SerializeField] private Material fadeInOut;

    private void Awake()
    {
        fadeInOut.SetFloat("_Fade", 1f);
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
        yield return new WaitForSeconds(0.5f);
        float time = 1f;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            fadeInOut.SetFloat("_Fade", time);
            yield return null;
        }
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
