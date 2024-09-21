using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Setting Data", menuName = "Data / Setting Data")]
public class GameSettingData : ScriptableObject
{
    public RewardData rewardData;
    public MapData mapData;
    public NameData nameData;
    public RankData rankData;
}

[Serializable]
public class RewardData
{
    public List<int> starRankReward;
    public List<int> starKillReward;
}

[Serializable]
public class MapData
{
    public List<Sprite> mapSprites;
}

[Serializable]
public class NameData
{
    public List<string> names;
}

[Serializable]
public class RankData
{
    public List<Rank> ranks;
}

[Serializable]
public class Rank
{
    public string name;
    public Sprite icon;
    public int maxStar;
    public string showText;
    public Sprite itemIcon;
}