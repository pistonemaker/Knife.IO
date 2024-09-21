using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public StarRank starRank;
    public ImageData imageData;
    public Button settingButton;
    public Button startButton;
    public Button diyButton;
    public Image newIcon;
    public Setting settingBox;
    public ChooseBox chooseBox;
    public GameSettingData settingData;
    public SpriteRenderer playerFace;
    public SpriteRenderer playerColor;
    public Map map;
    public GameObject player;
    public List<SpriteRenderer> knifes;

    [SerializeField] private RectTransform upPosUp;
    [SerializeField] private RectTransform upPosDown;
    [SerializeField] private RectTransform pPosUp;
    [SerializeField] private RectTransform pPosDown;
    [SerializeField] private RectTransform downPosUp;
    [SerializeField] private RectTransform downPosDown;

    private void OnEnable()
    {
        OnOpenGame();
        SetRandomMap();
        startButton.onClick.AddListener(LoadToSceneGame);
        settingButton.onClick.AddListener(ShowSetting);
        diyButton.onClick.AddListener(ShowChooseBox);
        starRank.Init();

        CheckNewFaceOrKnife();
        ResetPlayerSprites();
            
        this.RegisterListener(EventID.Face_Choosed, (param) => SetPlayerSprite((ImageFace) param));
        this.RegisterListener(EventID.Knife_Choosed, (param) => SetPlayerKnife((Sprite) param));
        EventDispatcher.Instance.RegisterListener(EventID.Diy_Show, OnDiyShow);
        EventDispatcher.Instance.RegisterListener(EventID.Diy_Hide, OnDiyHide);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        diyButton.onClick.RemoveAllListeners();
        
        this.RemoveListener(EventID.Face_Choosed, (param) => SetPlayerSprite((ImageFace) param));
        this.RemoveListener(EventID.Knife_Choosed, (param) => SetPlayerKnife((Sprite) param));
        EventDispatcher.Instance.RemoveListener(EventID.Diy_Show, OnDiyShow);
        EventDispatcher.Instance.RemoveListener(EventID.Diy_Hide, OnDiyHide);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        AudioManager.Instance.PlayMusic("Game_Play");
        OpenAppManager.Instance.CheckShowOpenAppAds();
        AdmobAds.Instance.ShowBannerAds();
    }

    private void CheckNewFaceOrKnife()
    {
        bool hasNew = false;
        
        for (int i = 0; i < imageData.faces.Count; i++)
        {
            if (!imageData.faces[i].hasClickedAfterUnlock && imageData.faces[i].isUnlock)
            {
                hasNew = true;
            }
        }
        for (int i = 0; i < imageData.knifes.Count; i++)
        {
            if (!imageData.knifes[i].hasClickedAfterUnlock && imageData.knifes[i].isUnlock)
            {
                hasNew = true;
            }
        }

        if (hasNew)
        {
            newIcon.gameObject.SetActive(true);
        }
        else
        {
            newIcon.gameObject.SetActive(false);
        }
    }

    private void ResetPlayerSprites()
    {
        SetPlayerKnife(imageData.knifes[PlayerPrefs.GetInt(DataKey.ID_CurKnifeType)].
            sprites[PlayerPrefs.GetInt(DataKey.ID_CurKnifeColor)]);
        SetPlayerSprite(imageData.faces[PlayerPrefs.GetInt(DataKey.ID_CurFace)]);
        SetPlayerColorRank(imageData.playerColorRanks[PlayerPrefs.GetInt(DataKey.ID_CurColorRank)]);
    }

    private void OnOpenGame()
    {
        int openGame = PlayerPrefs.GetInt(DataKey.Open_Count);
        PlayerPrefs.SetInt(DataKey.Open_Count, openGame + 1);
        
        int faceCount = imageData.faces.Count(face => face.isUnlock);
        PlayerPrefs.SetInt(DataKey.Face_Count, faceCount);
        
        int knifeCount = imageData.knifes.Count(knife => knife.isUnlock);
        PlayerPrefs.SetInt(DataKey.Knife_Count, knifeCount);

        if (PlayerPrefs.GetInt(DataKey.Login_Count) == 0)
        {
            PlayerPrefs.SetInt(DataKey.ID_CurKnifeColor, 0);
            PlayerPrefs.SetInt(DataKey.ID_CurKnifeType, 0);
            PlayerPrefs.SetInt(DataKey.ID_CurFace, 0);
            imageData.curFace = imageData.faces[0];
            imageData.curKnifeSprite = imageData.knifes[0].sprites[0];
        }
    }

    private void LoadToSceneGame()
    {
        PlayerNameInput.Instance.CheckSetName();
        SceneManager.LoadSceneAsync("Game");
    }

    private void ShowSetting()
    {
        settingBox.gameObject.SetActive(true);
    }

    private void ShowChooseBox()
    {
        EventDispatcher.Instance.PostEvent(EventID.Diy_Show);
        chooseBox.gameObject.SetActive(true);
    }

    private void SetPlayerSprite(ImageFace data)
    {
        StartCoroutine(ChangePlayerSprite(data));
    }
    
    private IEnumerator ChangePlayerSprite(ImageFace data)
    {
        playerFace.sprite = data.sprite1;
        yield return new WaitForSeconds(0.25f);
        playerFace.sprite = data.sprite2;
        yield return new WaitForSeconds(0.25f);
        playerFace.sprite = data.sprite1;
    }

    private void SetPlayerColorRank(ImageColorRank data)
    {
        playerColor.sprite = data.color;
        imageData.curColor = data.color;
        imageData.curRank = data.rank;
    }

    public void RandomPlayerColor()
    {
        int random = Random.Range(0, imageData.playerColorRanks.Count);
        PlayerPrefs.SetInt(DataKey.ID_CurColorRank, random);
        
        playerColor.sprite = imageData.playerColorRanks[random].color;
        imageData.curColor = playerColor.sprite;
        imageData.curRank = imageData.playerColorRanks[random].rank;
    }

    private void SetPlayerKnife(Sprite sprite)
    {
        for (int i = 0; i < knifes.Count; i++)
        {
            knifes[i].sprite = sprite;
        }
    }

    private void SetRandomMap()
    {
        int random = Random.Range(0, settingData.mapData.mapSprites.Count);
        PlayerPrefs.SetInt(DataKey.Map_ID, random);
        map.SetMap(settingData.mapData.mapSprites[random]);
    }

    private void OnDiyShow(object param)
    {
        starRank.transform.DOLocalMoveY(upPosUp.transform.localPosition.y, 0.75f);
        player.transform.DOLocalMoveY(pPosUp.transform.position.y, 0.75f);
        startButton.transform.DOLocalMoveY(downPosDown.transform.localPosition.y, 0.75f);
        diyButton.transform.DOLocalMoveY(downPosDown.transform.localPosition.y, 0.75f);
    }

    private void OnDiyHide(object param)
    {
        starRank.transform.DOLocalMoveY(upPosDown.transform.localPosition.y, 0.75f);
        player.transform.DOLocalMoveY(pPosDown.transform.position.y, 0.75f);
        startButton.transform.DOLocalMoveY(downPosUp.transform.localPosition.y, 0.75f);
        diyButton.transform.DOLocalMoveY(downPosUp.transform.localPosition.y, 0.75f);
        CheckNewFaceOrKnife();
        ResetPlayerSprites();
    }
}
