using UnityEngine;
using TMPro;

public class PlayerNameInput : Singleton<PlayerNameInput>
{
    public TextMeshPro playerName;
    public TMP_InputField nameInputField;
    private int maxNameLength = 12;
    private string defaultName = "Player";

    private void Start()
    {
        if (PlayerPrefs.HasKey(DataKey.Main_Player_Name))
        {
            string savedName = PlayerPrefs.GetString(DataKey.Main_Player_Name);
            playerName.text = savedName;
        }

        nameInputField.characterLimit = maxNameLength; 
        nameInputField.onEndEdit.AddListener(OnEndEditName);
    }

    private void OnEndEditName(string newName)
    {
        playerName.text = StandardName(newName);
        PlayerPrefs.SetString(DataKey.Main_Player_Name, playerName.text);
    }

    private string StandardName(string nameToStandard)
    {
        if (nameToStandard.Length <= maxNameLength)
        {
            return nameToStandard;
        }

        return nameToStandard.Substring(0, maxNameLength);
    }

    public void CheckSetName()
    {
        if (PlayerPrefs.GetString(DataKey.Main_Player_Name) == "")
        {
            playerName.text = defaultName;
            PlayerPrefs.SetString(DataKey.Main_Player_Name, defaultName);
        }
    }
}