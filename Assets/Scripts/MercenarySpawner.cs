using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenarySpawner : Singleton<MercenarySpawner>
{
    [SerializeField] private GameObject[] mercenaryPrefab;
    [SerializeField] protected List<GameObject> mercenaryList = new List<GameObject>();
    public GameObject next;
    public GameObject curMercenary;
    private bool isNext = true;

    private void Start()
    {
        for(int i = 0; i < mercenaryPrefab.Length;i++)
        {
            GameObject mercenary = Instantiate(mercenaryPrefab[i], transform);
            mercenaryList.Add(mercenary);
        }
    }

    public void NextMercenary()
    {
        
        if(isNext)
        {
            if (curMercenary == null)
            {
                RandomExitPool();
                next.SetActive(false);
                LobbyManager.instance.Gold--;
            }
            else
            {
                MercenaryController controller = curMercenary.GetComponent<MercenaryController>();

                if (controller.mercenaryState == MercenaryState.Sit)
                {
                    StartCoroutine(NextCo(controller));
                    next.SetActive(false);
                    LobbyManager.instance.Gold--;
                }
            }
        }
    }

    public void EnterPool(GameObject mercenary)
    {
        mercenary.gameObject.SetActive(false);
        mercenaryList.Add(mercenary);
    }    

    private void RandomExitPool()
    {
        int rand = Random.Range(0, mercenaryList.Count);
        curMercenary = mercenaryList[rand];
        mercenaryList[rand].SetActive(true);
        mercenaryList.RemoveAt(rand);
    }

    private IEnumerator NextCo(MercenaryController mercenaryController)
    {
        isNext = false;
        LobbyManager.instance.paper.SetActive(false);
        mercenaryController.animator.SetTrigger("Up");
        yield return new WaitForSeconds(2f);
        mercenaryController.posIndex++;
        mercenaryController.ChangeState(MercenaryState.Walk);
        RandomExitPool();
        isNext = true;
    }
}
