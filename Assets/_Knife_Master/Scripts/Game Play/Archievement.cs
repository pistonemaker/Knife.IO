using UnityEngine;

public class Archievement : Singleton<Archievement>
{
    public int numberOfGames;
    public int numberOfKilledEnermies;
    public int numberOfStars;
    public int numberOfLogins;
    public int numberOfFaces;
    public int numberOfKnifes;

    private void OnEnable()
    {
        LoadData();
        EventDispatcher.Instance.RegisterListener(EventID.New_Game_Start, HandleNewGameStart);
        EventDispatcher.Instance.RegisterListener(EventID.Player_Kill_An_Enermy, HandlePlayerKillAnEnermy);
        this.RegisterListener(EventID.Player_Gain_Star, (param) => HandlePlayerGainStar((int) param));
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.New_Game_Start, HandleNewGameStart);
        EventDispatcher.Instance.RemoveListener(EventID.Player_Kill_An_Enermy, HandlePlayerKillAnEnermy);
        this.RemoveListener(EventID.Player_Gain_Star, (param) => HandlePlayerGainStar((int) param));
    }

    private void LoadData()
    {
        numberOfGames = PlayerPrefs.GetInt(DataKey.Game_Count);
        numberOfKilledEnermies = PlayerPrefs.GetInt(DataKey.Enermy_Killed);
        numberOfStars = PlayerPrefs.GetInt(DataKey.Star);
        numberOfLogins = PlayerPrefs.GetInt(DataKey.Login_Count);
        numberOfFaces = PlayerPrefs.GetInt(DataKey.Face_Count);
        numberOfKnifes = PlayerPrefs.GetInt(DataKey.Knife_Count);
    }

    private void HandleNewGameStart(object param)
    {
        int gameCount = PlayerPrefs.GetInt(DataKey.Game_Count);
        numberOfGames = gameCount + 1;
        PlayerPrefs.SetInt(DataKey.Game_Count, numberOfGames);
    }

    private void HandlePlayerKillAnEnermy(object param)
    {
        int enermyKilled = PlayerPrefs.GetInt(DataKey.Enermy_Killed);
        numberOfKilledEnermies = enermyKilled + 1;
        PlayerPrefs.SetInt(DataKey.Enermy_Killed, numberOfKilledEnermies);
    }

    private void HandlePlayerGainStar(int starGain)
    {
        int curStar = PlayerPrefs.GetInt(DataKey.Star);
        numberOfStars = curStar + starGain;
        PlayerPrefs.SetInt(DataKey.Star, numberOfStars);
    }

    public int AmountProgress(UnlockType unlockType)
    {
        switch (unlockType)
        {
            case UnlockType.Play_Game:
            {
                return numberOfGames;
            }
            case UnlockType.Kill_Enermy:
            {
                return numberOfKilledEnermies;
            }
            case UnlockType.Gain_Star:
            {
                return numberOfStars;
            }
            case UnlockType.Login:
            {
                return numberOfLogins;
            }
            case UnlockType.Watch_AD:
            {
                return 0;
            }
            case UnlockType.Gain_Face:
            {
                return numberOfFaces;
            }
            case UnlockType.Gain_Knife:
            {
                return numberOfKnifes;
            }
            default:
            {
                return 0;
            }
        }
    }

    public string FormatUnlockCondition(UnlockType unlockType, int amountToUnlock)
    {
        switch (unlockType)
        {
            case UnlockType.Play_Game:
            {
                return "Play " + amountToUnlock + " game to unlock";
            }
            case UnlockType.Kill_Enermy:
            {
                return "Kill " + amountToUnlock + " enermy to unlock";
            }
            case UnlockType.Gain_Star:
            {
                return "Gain " + amountToUnlock + " star to unlock";
            }
            case UnlockType.Login:
            {
                return "Login " + amountToUnlock + " days to unlock";
            }
            case UnlockType.Watch_AD:
            {
                return "Watch AD " + amountToUnlock + " times to unlock";
            }
            case UnlockType.Gain_Face:
            {
                return "Collect " + amountToUnlock + " faces to unlock";
            }
            case UnlockType.Gain_Knife:
            {
                return "Collect " + amountToUnlock + " knifes to unlock";
            }
            default:
            {
                return "";
            }
        }
    }
}

public enum UnlockType
{
    Play_Game,
    Kill_Enermy,
    Gain_Star,
    Login,
    Watch_AD,
    Gain_Knife,
    Gain_Face,
}
