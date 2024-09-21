using UnityEngine;
using System;

public class LoginManager : Singleton<LoginManager>
{
    private void Start()
    {
        if (IsTodayFirstLogin())
        {
            IncreaseLoginCount();
            DataKey.UpdateLastLoginDate();
            Debug.Log("Logined");
        }
        else
        {
            Debug.Log("Already logged in today.");
        }
    }

    private bool IsTodayFirstLogin()
    {
        string lastLoginDateString = PlayerPrefs.GetString(DataKey.Last_Login_Date, string.Empty);
        
        DateTime lastLoginDate;
        if (DateTime.TryParse(lastLoginDateString, out lastLoginDate))
        {
            return lastLoginDate.Date != DateTime.Today;
        }
        
        return true;
    }

    private void IncreaseLoginCount()
    {
        int currentCount = PlayerPrefs.GetInt(DataKey.Login_Count, 0);
        PlayerPrefs.SetInt(DataKey.Login_Count, currentCount + 1);
    }
}