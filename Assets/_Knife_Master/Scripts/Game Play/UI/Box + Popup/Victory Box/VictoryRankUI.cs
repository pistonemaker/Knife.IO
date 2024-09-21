using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryRankUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI score;
    public Image image;

    public void Init(Player player, int id)
    {
        rankText.text = "Rank " + id;
        playerName.text = player.name;
        score.text = player.knifeAmount.ToString();
        
        if (id == 1)
        {
            rankText.color = Color.yellow;
            rankText.fontSize = 40f;
            image.sprite = VictoryBox.Instance.winSprite;
        }
        else
        {
            image.sprite = VictoryBox.Instance.loseSprite;
        }
    }
}
