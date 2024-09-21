using System.Collections.Generic;
using UnityEngine;

public class BattleRankUIManager : Singleton<BattleRankUIManager>
{
    [SerializeField] private Transform content;
    [SerializeField] private BattleRankUI rankUIPrefab;
    [SerializeField] private List<BattleRankUI> rankUis;

    public void Init()
    {
        var listPlayer = GameManager.Instance.players;
        for (int i = 0; i < listPlayer.Count; i++)
        {
            var rankUI = PoolingManager.Spawn(rankUIPrefab, content.transform.position, Quaternion.identity);
            rankUI.transform.SetParent(content);
            rankUI.transform.SetSiblingIndex(i);
            rankUI.transform.localScale = Vector3.one;
            rankUI.playerIcon.sprite = listPlayer[i].rankSprite;
            rankUI.playerNameText.text = listPlayer[i].playerName.text;
            rankUI.knifeAmountText.text = listPlayer[i].knifeAmount.ToString();
            rankUI.player = listPlayer[i];
            listPlayer[i].rankUI = rankUI;
            rankUis.Add(rankUI);
        }
    }
    
    public void UpdateRank()
    {
        rankUis.Sort((a, b) => 
            b.player.knifeAmount.CompareTo(a.player.knifeAmount));

        for (int i = 0; i < rankUis.Count; i++)
        {
            rankUis[i].transform.SetSiblingIndex(i);
        }
    }
}
