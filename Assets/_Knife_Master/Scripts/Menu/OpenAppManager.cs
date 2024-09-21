using UnityEngine;

public class OpenAppManager : MonoBehaviour
{
    public static OpenAppManager Instance;
    public bool canShowOpenAppAds = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckShowOpenAppAds()
    {
        if (canShowOpenAppAds)
        {
            AdmobAds.Instance.ShowAppOpenAds();
        }
    }

    private void Start()
    {
        int openAppCount = PlayerPrefs.GetInt(DataKey.Open_App_Count);
        PlayerPrefs.SetInt(DataKey.Open_App_Count, openAppCount + 1);
    }
}
