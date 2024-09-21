using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "Data / Game Data")]
public class GameData : ScriptableObject
{
    public List<EnermyData> enermyData;
    public List<GameObject> obstaclePrefabs;
    public GameObject enermyPrefab;
    public GameObject rocketPrefab;
    public GameObject blackHolePrefab;
}

[Serializable]
public class EnermyData
{
    public Vector2 spawnPos;
    public Sprite sprite;
    public Sprite rankSprite;
}