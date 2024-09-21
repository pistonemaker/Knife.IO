using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarRank : MonoBehaviour
{
    [SerializeField] private Image process;
    [SerializeField] private Button showRankButton;
    [SerializeField] private TextMeshProUGUI rankName;
    [SerializeField] private TextMeshProUGUI starText;
    private int star;
    private int maxStar;
    private int id;
    [SerializeField] private RankPopup rankPopup;

    public void Init()
    {
        GetData();
        SetProcess();
        showRankButton.onClick.AddListener(ShowRankPopup);
    }

    public void GetData()
    {
        var listRank = MenuManager.Instance.settingData.rankData.ranks;
        star = PlayerPrefs.GetInt(DataKey.Star);

        for (int i = 0; i < listRank.Count; i++)
        {
            if (star < listRank[i].maxStar)
            {
                id = i;
                maxStar = listRank[i].maxStar;
                showRankButton.image.sprite = listRank[i].icon;
                rankName.text = listRank[i].name;
                starText.text = star + "/" + listRank[i].maxStar;
                PlayerPrefs.SetString(DataKey.Current_Rank, listRank[i].name);
                break;
            }
        }

        if (star > listRank[listRank.Count - 1].maxStar)
        {
            id = listRank.Count - 1;
            maxStar = listRank[listRank.Count - 1].maxStar;
            showRankButton.image.sprite = listRank[listRank.Count - 1].icon;
            rankName.text = listRank[listRank.Count - 1].name;
            starText.text = star + "/" + listRank[listRank.Count - 1].maxStar;
            PlayerPrefs.SetString(DataKey.Current_Rank, listRank[listRank.Count - 1].name);
        }
    }

    public void SetProcess()
    {
        float rate = (float)star / maxStar;
        if (rate > 1f)
        {
            rate = 1f;
        }

        process.DOFillAmount(rate, 0.25f);
    }

    private void ShowRankPopup()
    {
        rankPopup.gameObject.SetActive(true);
        SetRankPopupData(id);
    }

    private void SetRankPopupData(int id)
    {
        rankPopup.Init(id, star);
    }
}