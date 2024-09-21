using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleRankUI : MonoBehaviour
{
    public Image playerIcon;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI knifeAmountText;
    public Player player;

    public void UpdateKnifeAmount()
    {
        if (player.knifeAmount < 0)
        {
            player.knifeAmount = 0;
        }
        playerNameText.text = player.playerName.text;
        knifeAmountText.text = player.knifeAmount.ToString();
        BattleRankUIManager.Instance.UpdateRank();
    }
}
