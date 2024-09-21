using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    public TextMeshPro playerName;
    
    private void Start()
    {
        playerName.text = PlayerPrefs.GetString(DataKey.Main_Player_Name);
    }
}
