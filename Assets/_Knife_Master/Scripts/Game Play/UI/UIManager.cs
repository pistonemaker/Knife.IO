using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameSettingData settingData;
    public KillAmountBox killAmountBox;
    public KillAnnoucement killAnnoucement;

    public VictoryBox victoryBox;
    public LoseBox loseBox;
    [SerializeField] private List<Player> rankingList;

    private Coroutine killUICoroutine;

    [SerializeField] private Transform startPos, midPos, endPos;

    private void OnEnable()
    {
        this.RegisterListener(EventID.Player_Die, (param) => Handle1PlayerDie((Player)param));
        this.RegisterListener(EventID.Main_Player_Die, (param) => OnPlayerDie((Player)param));
        EventDispatcher.Instance.RegisterListener(EventID.Main_Player_Win, OnPlayerWin);
    }

    private IEnumerator ShowKillUI(Player killed)
    {
        if (killUICoroutine != null)
            StopCoroutine(killUICoroutine);
        
        killAnnoucement.killAnnoucementText.text = killed.killer.playerName.text + " killed " + killed.name;
        
        // Nếu người hạ gục là người chơi thì bật kill popup
        if (killed.killer == GameManager.Instance.mainPlayer)
        {
            EventDispatcher.Instance.PostEvent(EventID.Player_Kill_An_Enermy);
            killAmountBox.ShowBox(killed);
        }
        killAnnoucement.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        
        killAnnoucement.gameObject.SetActive(false);

        killAmountBox.gameObject.SetActive(false);
        killUICoroutine = null; 
    }

    public void Handle1PlayerDie(object param)
    {
        killUICoroutine = StartCoroutine(ShowKillUI((Player)param));
    }

    private void OnPlayerWin(object param)
    {
        victoryBox.gameObject.SetActive(true);
        ShowRankUI();
    }

    private void OnPlayerDie(object param)
    {
        loseBox.gameObject.SetActive(true);
        ShowLoseUI((Player) param);
    }
    
    public void ShowRankUI()
    {
        GetRanking();
    }
    
    private void GetRanking()
    {
        GameManager.Instance.playerRanking.Add(GameManager.Instance.mainPlayerGO.GetComponent<Player>());
        var count = GameManager.Instance.playerRanking.Count;
        for (int i = count - 1; i >= count - 6; i--)
        {
            rankingList.Add(GameManager.Instance.playerRanking[i]);
        }
        ShowRankingAndStarGain(rankingList[0]);
    }

    private void ShowRankingAndStarGain(Player player)
    {
        victoryBox.ShowRanking(rankingList);
        victoryBox.ShowStarGain(player, player.rank, player.killAmount);
    }
    
    private void ShowLoseUI(Player player)
    {
        loseBox.ShowLoseInfo(player);
        loseBox.ShowStarGain(player, player.rank, player.killAmount);
    }
}