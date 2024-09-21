using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffStar : MonoBehaviour
{
    public StarRank starRank;
    public Button buffButton;
    public TMP_InputField buffIPF;
    private int starToBuff = 0;

    private void OnEnable()
    {
        buffIPF.onEndEdit.AddListener(InputStar);
        buffButton.onClick.AddListener(Buff);
    }
    
    private void OnDisable()
    {
        buffIPF.onEndEdit.RemoveAllListeners();
        buffButton.onClick.RemoveAllListeners();
    }

    private void Buff()
    {
        int star = PlayerPrefs.GetInt(DataKey.Star);
        PlayerPrefs.SetInt(DataKey.Star, star + starToBuff);
        starRank.GetData();
        starRank.SetProcess();
    }

    private void InputStar(string starStr)
    {
        starToBuff = Convert.ToInt32(starStr);
    }
}
