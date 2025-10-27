using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject[] textPrefab;
    private Stack<TextMeshProUGUI> basicStack = new Stack<TextMeshProUGUI>();
    private Stack<TextMeshProUGUI> criticalStack = new Stack<TextMeshProUGUI>();
    [SerializeField] private Transform textPos;

    private void Awake()
    {
        for (int i = 0; i < textPrefab.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                TextMeshProUGUI damageText = Instantiate(textPrefab[i], transform).GetComponent<TextMeshProUGUI>();
                if (i == 0)
                {
                    basicStack.Push(damageText);
                }
                else
                {
                    criticalStack.Push(damageText);
                }
            }
        }
    }

    public void EnterPool(TextMeshProUGUI text, TextType textType)
    {
        if (textType == TextType.Normal)
        {
            basicStack.Push(text);
        }
        else
        {
            criticalStack.Push(text);
        }

        text.gameObject.SetActive(false);
    }

    public void ShowDamageText(float damage, TextType textType)
    {
        TextMeshProUGUI damageText;

        if (textType == TextType.Normal)
        {
            Refill(5, 0, basicStack);
            damageText = basicStack.Pop();
        }
        else
        {
            Refill(5, 1, criticalStack);
            damageText = criticalStack.Pop();
        }
            
        if (damage >= 1_000_000)
        {
            damageText.text = (damage / 1000000).ToString("0.#") + "M";
        }
        else if (damage >= 1_000)
        {
            damageText.text = (damage / 1000).ToString("0.#") + "K";
        }
        else
        {
            damageText.text = damage.ToString("0");
        }
        damageText.transform.position = textPos.position;
        damageText.transform.DOKill();
        damageText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        damageText.gameObject.SetActive(true);
        damageText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
            {
                EnterPool(damageText, textType);
            });
        });
    }

    private void Refill(int count, int prefabIndex, Stack<TextMeshProUGUI> targetStack)
    {
        for (int i = 0; i < count; i++)
        {
            var damageText = Instantiate(textPrefab[prefabIndex], transform).GetComponent<TextMeshProUGUI>();
            damageText.transform.localScale = Vector3.one * 0.1f;
            targetStack.Push(damageText);
        }
    }

}
