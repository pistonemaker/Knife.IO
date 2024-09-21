using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBox : Singleton<ChooseBox>
{
    public Sprite character;
    public Sprite nonCharacter;
    public Sprite knife;
    public Sprite nonKnife;

    public Button closeButton;
    public Button characterButton;
    public Button knifeButton;

    public KnifeChoosing knifeChoosing;
    public FaceChoosing faceChoosing;
    
    public Sprite locked;
    public Sprite unlocked;

    public Image progressLock;
    public Image progressUnlock;
    
    public TextMeshProUGUI unlockText;
    public TextMeshProUGUI progressText;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        characterButton.image.sprite = nonCharacter;
        knifeButton.image.sprite = knife;
        knifeChoosing.Init();
        faceChoosing.Init();
        knifeChoosing.gameObject.SetActive(true);
        faceChoosing.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        characterButton.onClick.AddListener(ShowFaceChoosing);
        
        knifeButton.onClick.AddListener(ShowKnifeChoosing);
        
        closeButton.onClick.AddListener(CloseFaceChoosing);
    }

    private void ShowKnifeChoosing()
    {
        if (knifeButton.image.sprite == nonKnife)
        {
            knifeButton.image.sprite = knife;
            characterButton.image.sprite = nonCharacter;
            faceChoosing.gameObject.SetActive(false);
            knifeChoosing.gameObject.SetActive(true);
        }
    }

    private void ShowFaceChoosing()
    {
        if (characterButton.image.sprite == nonCharacter)
        {
            characterButton.image.sprite = character;
            knifeButton.image.sprite = nonKnife;
            faceChoosing.gameObject.SetActive(true);
            knifeChoosing.gameObject.SetActive(false);
        }
    }

    private void CloseFaceChoosing()
    {
        gameObject.SetActive(false);
        EventDispatcher.Instance.PostEvent(EventID.Diy_Hide);
    }
}
