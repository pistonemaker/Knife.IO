using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class VictoryBox : Singleton<VictoryBox>
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private VictoryRankUI victoryRankUIPrefab;
    [SerializeField] private StarGain starGain;
    [SerializeField] private CollectButton tripleCollectButton;
    [SerializeField] private CollectButton collectButton;
    public Sprite winSprite;
    public Sprite loseSprite;

    private void OnEnable()
    {
        ShowBox();
    }

    private void ShowBox()
    {
        StartCoroutine(PlayerWinAction());
    }

    private IEnumerator PlayerWinAction()
    {
        yield return new WaitForSeconds(1f);
        transform.position = startPos.position;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(endPos.position.y, 1f);
    }

    public void ShowRanking(List<Player> rankingList)
    {
        for (int i = 0; i < rankingList.Count; i++)
        {
            var victoryRankUI = PoolingManager.Spawn(victoryRankUIPrefab, transform.position, Quaternion.identity);
            victoryRankUI.Init(rankingList[i], i + 1);
            var trf = victoryRankUI.transform;
            trf.SetParent(content.transform);
            trf.SetSiblingIndex(i);
            trf.localScale = Vector3.one;
        }
    }

    public void ShowStarGain(Player player, int rank, int kills)
    {
        var gameSettingData = UIManager.Instance.settingData;
        var rankStar = gameSettingData.rewardData.starRankReward[rank];
        var killStar = gameSettingData.rewardData.starKillReward[kills];
        starGain.Init(rankStar, player.killAmount, killStar, 0, 0);

        var total = rankStar + killStar + 0 + 0;
        collectButton.Init(total, false);
        tripleCollectButton.Init(3 * total, true);
    }
}
