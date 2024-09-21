using System;
using UnityEngine;

public static class DataKey
{
    #region int
    
    public const string Star = "Star";
    public const string ID_CurFace = "ID_CurFace";
    public const string ID_CurColorRank = "ID_CurColor";
    public const string ID_CurKnifeType = "ID_CurKnifeType";
    public const string ID_CurKnifeColor = "ID_CurKnifeColor";
    public const string Use_Vibrate = "Use_Vibrate";
    public const string Use_Music = "Use_Music";
    public const string Map_ID = "Map_ID";
    public const string Game_Count = "Game_Count";
    public const string Enermy_Killed = "Enermy_Killed";
    public const string Login_Count = "Login_Count";
    public const string Watch_AD_Count = "Watch_AD_Count";
    public const string Face_Count = "Face_Count";
    public const string Knife_Count = "Knife_Count";
    public const string Open_Count = "Open_Count";
    public const string Open_App_Count = "Open_App_Count";

    #endregion
    
    #region string

    public const string Main_Player_Name = "Main_Player_Name";
    public const string Current_Rank = "Current_Rank";
    public const string Last_Login_Date = "Last_Login_Date";

    #endregion
    
    public static bool IsUseVibrate()
    {
        return PlayerPrefs.GetInt(Use_Vibrate) == 1;
    }

    public static bool IsUseMusic()
    {
        return PlayerPrefs.GetInt(Use_Music) == 1;
    }
    
    public static void UpdateLastLoginDate()
    {
        PlayerPrefs.SetString(DataKey.Last_Login_Date, DateTime.Today.ToString("yyyy-MM-dd"));
    }
}
