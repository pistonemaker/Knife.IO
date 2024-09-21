using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI starText;

    public void Init(int starGet, bool hasReward)
    {
        starText.text = "+" + starGet;
        
        button.onClick.AddListener(() =>
        {
            button.interactable = false;
            this.PostEvent(EventID.Player_Gain_Star, starGet);
            
            if (!hasReward)
            {
                int watchAdCount = PlayerPrefs.GetInt(DataKey.Watch_AD_Count);
                PlayerPrefs.SetInt(DataKey.Watch_AD_Count, watchAdCount + 1);
                if (watchAdCount % 3 == 0)
                {
                    AdmobAds.Instance.ShowInterAds(LoadMenuScene);
                }
                else
                {
                    LoadMenuScene();
                }
            }
            else
            {
                AdmobAds.Instance.ShowRewardAds(LoadMenuScene);
            }
        });
    }

    private void LoadMenuScene()
    {
        DOTween.Clear();
        SceneManager.LoadSceneAsync("Menu");
    }
}
