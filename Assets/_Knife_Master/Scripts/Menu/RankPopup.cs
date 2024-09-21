using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankPopup : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Image rankIcon;
    [SerializeField] private Image process;
    [SerializeField] private TextMeshProUGUI rankName;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI showText;
    [SerializeField] private Image iconImage;
    private int curID;
    private int curStar;

    public void Init(int id, int star)
    {
        curID = id;
        curStar = star;
        var ranks = MenuManager.Instance.settingData.rankData.ranks;
        LoadData(ranks);
        
        leftButton.onClick.AddListener(LoadPrevRank);
        rightButton.onClick.AddListener(LoadNextRank);
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        
        SetProcess(id, star);
    }

    private void OnDisable()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void LoadNextRank()
    {
        var ranks = MenuManager.Instance.settingData.rankData.ranks;
        curID++;
        
        if (curID == ranks.Count - 1)
        {
            rightButton.gameObject.SetActive(false);
        }

        if (curID > 0)
        {
            leftButton.gameObject.SetActive(true);
        }
        
        LoadData(ranks);
        
        SetProcess(curID, curStar);
    }

    private void LoadPrevRank()
    {
        var ranks = MenuManager.Instance.settingData.rankData.ranks;
        curID--;
        
        if (curID == 0)
        {
            leftButton.gameObject.SetActive(false);
        }

        if (curID < ranks.Count - 1)
        {
            rightButton.gameObject.SetActive(true);
        }
        
        LoadData(ranks);
        
        SetProcess(curID, curStar);
    }

    private void LoadData(List<Rank> ranks)
    {
        rankIcon.sprite = ranks[curID].icon;
        rankName.text = ranks[curID].name;
        starText.text = PlayerPrefs.GetInt(DataKey.Star) + "/" + ranks[curID].maxStar;

        showText.text = ranks[curID].showText;
        
        if (ranks[curID].itemIcon != null)
        {
            iconImage.sprite = ranks[curID].itemIcon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }
    
    private void SetProcess(int id, int star)
    {
        var sizeDelta = process.rectTransform.sizeDelta;
        float maxStar = MenuManager.Instance.settingData.rankData.ranks[id].maxStar;
        float maxSize = 800f;
        float curSize = (star / maxStar) * maxSize;
        
        if (curStar > maxStar)
        {
            curSize = maxSize;
        }

        process.rectTransform.sizeDelta = new Vector2(0f, sizeDelta.y);
        process.rectTransform.DOSizeDelta(new Vector2(curSize, sizeDelta.y), 1f);
    }
}
