using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int stt = 1;
    [SerializeField] private int knifeInit;
    [SerializeField] private int knifeSpawn;
    [SerializeField] private float maxHeightOfMap;
    [SerializeField] private float maxWidthOfMap;
    [SerializeField] private float minDistanceBetweenPlayers;
    [SerializeField] private int maxAttemptsToFindSpawnPosition;

    public float cameraSizeMoving = 16f;
    private int playerCount;

    private Transform knifesParent;
    private Transform obstaclesParent;
    private Transform itemParent;
    public GameObject knifePrefab;
    public GameObject mainPlayerGO;
    public Player mainPlayer;
    public Map map;

    [SerializeField] private GameData gameData;
    [SerializeField] private ImageData imageData;
    [SerializeField] private GameSettingData settingData;

    [SerializeField] private List<GameObject> knifes;
    public List<Player> players;
    public List<Player> playerRanking;

    private void Start()
    {
        Application.targetFrameRate = 60;
        map.SetMap(settingData.mapData.mapSprites[PlayerPrefs.GetInt(DataKey.Map_ID)]);

        mainPlayer = mainPlayerGO.GetComponent<Player>();
        mainPlayer.SetFaceAndKnife(imageData.curFace, imageData.curKnifeSprite);
        mainPlayer.CreateKnifeInit(3);
        mainPlayer.ResetDataRespawn();
        players.Add(mainPlayer);

        knifesParent = transform.Find("Knifes Parent").transform;
        obstaclesParent = transform.Find("Obstacles Parent").transform;
        itemParent = transform.Find("Item Parent").transform;

        SpawnEnermies();
        SpawnRandomKnifes(knifeInit);
        CheckCanSpawnItem();
        StartCoroutine(SpawnKnifesRoutine());
        StartCoroutine(SpawnEnermyRoutine());
        EventDispatcher.Instance.PostEvent(EventID.New_Game_Start);
        AdmobAds.Instance.interstitialAdController.LoadAd();
        AdmobAds.Instance.rewardedAdController.LoadAd();
        AdmobAds.Instance.ShowBannerAds();
    }

    private void SpawnEnermies()
    {
        for (int i = 0; i < gameData.enermyData.Count; i++)
        {
            int random = Random.Range(0, imageData.faces.Count);
            int random2 = Random.Range(0, imageData.knifes.Count);
            int random3 = Random.Range(0, imageData.knifes[random2].sprites.Count);
            var enermy = PoolingManager.Spawn(gameData.enermyPrefab, gameData.enermyData[i].spawnPos,
                Quaternion.identity);

            var player = enermy.GetComponent<Player>();
            player.SetFaceAndKnife(imageData.faces[random], imageData.knifes[random2].sprites[random3]);
            player.CreateKnifeInit(3);
            player.ResetDataRespawn();
            player.playerName.text = GetRandomName();
            player.name = player.playerName.text;
            players.Add(player);

            var sr = enermy.GetComponent<SpriteRenderer>();
            sr.sprite = gameData.enermyData[i].sprite;
            player.rankSprite = gameData.enermyData[i].rankSprite;
        }

        playerCount = players.Count;
        BattleRankUIManager.Instance.Init();

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == mainPlayer)
            {
                mainPlayer.SetColorAndRankSprite(imageData.curColor, imageData.curRank);
            }
            else
            {
                int random = Random.Range(0, imageData.playerColorRanks.Count);
                players[i].SetColorAndRankSprite(imageData.playerColorRanks[random].color,
                    imageData.playerColorRanks[random].rank);
            }

            players[i].rankUI.UpdateKnifeAmount();
        }
    }

    private IEnumerator SpawnKnifesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (knifes.Count < knifeInit)
            {
                SpawnRandomKnifes(knifeSpawn);
            }
        }
    }

    private string GetRandomName()
    {
        return settingData.nameData.names[Random.Range(0, settingData.nameData.names.Count)];
    }

    public void SpawnRandomKnifes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float x = Random.Range(-maxWidthOfMap, maxHeightOfMap);
            float y = Random.Range(-maxWidthOfMap, maxHeightOfMap);

            var knife = PoolingManager.Spawn(knifePrefab, new Vector3(x, y, 0f), Quaternion.identity);
            knife.transform.SetParent(knifesParent);
            knife.name = "Knife " + stt++;
            knifes.Add(knife);
        }
    }

    private IEnumerator SpawnEnermyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            if (playerRanking.Count > 0)
            {
                SpawnRandomEnermy();
            }
        }
    }

    private void SpawnRandomEnermy()
    {
        // Tìm vị trí phù hợp để sinh ra người chơi mới
        Vector3 spawnPosition = FindValidSpawnPosition();

        int random = Random.Range(0, playerRanking.Count);
        var enermyToSpawn = playerRanking[random];

        if (!players.Contains(enermyToSpawn) && playerRanking.Contains(enermyToSpawn) && enermyToSpawn != mainPlayer)
        {
            enermyToSpawn.gameObject.SetActive(true);
            enermyToSpawn.transform.position = spawnPosition;
            enermyToSpawn.playerName.text = GetRandomName();
            enermyToSpawn.name = enermyToSpawn.playerName.text;
            players.Add(enermyToSpawn);
            playerRanking.Remove(enermyToSpawn);
            InitNewEnermyKnifes(enermyToSpawn);
            enermyToSpawn.ResetDataRespawn();
            enermyToSpawn.rankUI.UpdateKnifeAmount();
            SetRank();
        }
    }

    private void InitNewEnermyKnifes(Player newEnermy)
    {
        if (mainPlayer.rank == 0 || mainPlayer.rank == 1 || mainPlayer.rank == 2)
        {
            newEnermy.CreateKnifeInit(mainPlayer.knifeAmount - 2);
        }
        else
        {
            newEnermy.CreateKnifeInit(3);
        }
        
        newEnermy.ResetDataRespawn();
    }

    private void CheckCanSpawnItem()
    {
        if (PlayerPrefs.GetInt(DataKey.Star) >= settingData.rankData.ranks[1].maxStar)
        {
            SpawnObstacles();
        }

        if (PlayerPrefs.GetInt(DataKey.Star) >= settingData.rankData.ranks[2].maxStar)
        {
            SpawnRockets(3);
            StartCoroutine(SpawnRandomRocket());
        }

        if (PlayerPrefs.GetInt(DataKey.Star) >= settingData.rankData.ranks[3].maxStar)
        {
            StartCoroutine(SpawnBlackHole());
        }
    }

    private void SpawnObstacles()
    {
        int number = Random.Range(0, gameData.obstaclePrefabs.Count / 2);

        for (int i = 0; i < number; i++)
        {
            float x = Random.Range(-maxWidthOfMap, maxWidthOfMap);
            float y = Random.Range(-maxHeightOfMap, maxHeightOfMap);

            var obstacle = PoolingManager.Spawn(gameData.obstaclePrefabs
                [Random.Range(0, gameData.obstaclePrefabs.Count)], new Vector3(x, y, 0f), Quaternion.identity);
            obstacle.transform.SetParent(obstaclesParent);
        }
    }

    private IEnumerator SpawnRandomRocket()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            SpawnRockets(1);
        }
    }

    private void SpawnRockets(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float x = Random.Range(-maxWidthOfMap, maxWidthOfMap);
            float y = Random.Range(-maxHeightOfMap, maxHeightOfMap);

            var rocket = PoolingManager.Spawn(gameData.rocketPrefab, new Vector3(x, y, 0f),
                Quaternion.identity);
            rocket.transform.SetParent(itemParent);
        }
    }
    
    private IEnumerator SpawnBlackHole()
    {
        yield return new WaitForSeconds(15f);
        float x = Random.Range(-maxWidthOfMap, maxWidthOfMap);
        float y = Random.Range(-maxHeightOfMap, maxHeightOfMap);

        var blackHole = PoolingManager.Spawn(gameData.blackHolePrefab, new Vector3(x, y, 0f),
            Quaternion.identity);
        blackHole.transform.SetParent(itemParent);
    }

    public void RemoveKnife(GameObject knife)
    {
        if (knifes.Contains(knife))
        {
            knifes.Remove(knife);
        }
    }

    public void AddKnife(GameObject knife)
    {
        if (!knifes.Contains(knife))
        {
            knifes.Add(knife);
            knife.transform.SetParent(knifesParent);
        }
    }

    public GameObject GetRandomKnife()
    {
        for (int i = 0; i < knifes.Count; i++)
        {
            if (knifes[i] == null)
            {
                knifes.Remove(knifes[i]);
                i--;
            }
        }

        int random = Random.Range(0, knifes.Count);

        if (knifes[random] != null)
        {
            var knife = knifes[random].GetComponent<Knife>();
            if (knife != null && knife.canCollect)
            {
                return knifes[random];
            }

            for (int i = 0; i < knifes.Count; i++)
            {
                knife = knifes[i].GetComponent<Knife>();
                if (knife != null && knife.canCollect)
                {
                    return knifes[i];
                }
            }
        }

        SpawnRandomKnifes(5);
        return knifes[0];
    }

    public bool HasKnife()
    {
        return knifes.Count > 0;
    }

    public bool IsContainKnife(GameObject knife)
    {
        if (knifes.Contains(knife))
        {
            return true;
        }

        return false;
    }

    private void CheckIfSpawnMoreEnermy()
    {
        if (playerRanking.Count >= 5)
        {
            SpawnRandomEnermy();
        }
    }

    public void HandlePlayerDie(Player player)
    {
        if (players.Contains(player))
        {
            player.gameObject.SetActive(false);
            players.Remove(player);
        }

        if (!playerRanking.Contains(player))
        {
            playerRanking.Add(player);
            SetRank();
        }

        CheckIfSpawnMoreEnermy();
    }
    
    private void SetRank()
    {
        var count = playerRanking.Count;
        for (int i = 0; i < count; i++)
        {
            playerRanking[i].rank = playerCount - i - 1;
        }
    }

    private Vector3 FindValidSpawnPosition()
    {
        Vector3 spawnPosition;

        for (int i = 0; i < maxAttemptsToFindSpawnPosition; i++)
        {
            float x = Random.Range(-maxWidthOfMap, maxWidthOfMap);
            float y = Random.Range(-maxHeightOfMap, maxHeightOfMap);
            spawnPosition = new Vector3(x, y, 0f);
            bool isValidPosition = IsSpawnPositionValid(spawnPosition);

            if (isValidPosition)
            {
                return spawnPosition;
            }
        }

        Vector3[] corner =
        {
            new Vector3(41f, 41f, 0f), new Vector3(-41f, -41f, 0f),
            new Vector3(-41f, 41f, 0f), new Vector3(41f, -41f, 0f)
        };
        int random = Random.Range(0, corner.Length);

        return corner[random];
    }

    private bool IsSpawnPositionValid(Vector3 position)
    {
        foreach (var player in players)
        {
            float distance = Vector3.Distance(position, player.transform.position);

            if (distance < minDistanceBetweenPlayers)
            {
                return false;
            }
        }

        foreach (var player in playerRanking)
        {
            float distance = Vector3.Distance(position, player.transform.position);

            if (distance < minDistanceBetweenPlayers)
            {
                return false;
            }
        }

        return true;
    }
}