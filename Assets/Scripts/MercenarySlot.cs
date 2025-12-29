using TMPro;
using UnityEngine;

public class MercenarySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] statsText;
    public GameObject stateParent;

    public void AddMercenary(Mercenary mercenary)
    {
        statsText[0].text = mercenary.atk.ToString("N0");
        float safeAtkSpeed = Mathf.Max(0.1f, mercenary.atkSpeed);
        statsText[1].text = ((1 / safeAtkSpeed) * 10).ToString("N0");
        statsText[2].text = mercenary.def.ToString("N0");
        statsText[3].text = mercenary.hp.ToString("N0");
        statsText[4].text = (mercenary.criticalPercent * 100).ToString("N0") + "%";
        statsText[5].text = (mercenary.criticalDamage * 100).ToString("N0") + "%";
        statsText[6].text = (mercenary.skillDamage * 100).ToString("N0") + "%";
        statsText[7].text = mercenary.speed.ToString("N0");
        statsText[8].text = mercenary.mercenaryRank.ToString();
    }
}
