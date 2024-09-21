using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoseBox : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI killerName;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private StarGain starGain;
    [SerializeField] private CollectButton tripleCollectButton;
    [SerializeField] private CollectButton collectButton;

    private void OnEnable()
    {
        ShowBox();
    }

    private void ShowBox()
    {
        StartCoroutine(PlayerLoseAction());
    }

    private IEnumerator PlayerLoseAction()
    {
        yield return new WaitForSeconds(1f);
        transform.position = startPos.position;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(endPos.position.y, 1f);
    }
    
    public void ShowLoseInfo(Player player)
    {
        playerName.text = player.playerName.text;
        killerName.text = player.killer.name;
        rankText.text = "Rank " + (player.rank + 1);
    }
    
    public void ShowStarGain(Player player, int rank, int kills)
    {
        var rewardData = UIManager.Instance.settingData.rewardData;
        
        if (kills >= rewardData.starKillReward.Count)
        {
            kills = rewardData.starKillReward.Count - 1;
        }
        
        var rankStar = rewardData.starRankReward[rank];
        var killStar = rewardData.starKillReward[kills];
        starGain.Init(rankStar, player.killAmount, killStar, 0, 0);
        
        var total = rankStar + killStar + 0 + 0;
        collectButton.Init(total, false);
        tripleCollectButton.Init(total * 3, true);
    }
}
