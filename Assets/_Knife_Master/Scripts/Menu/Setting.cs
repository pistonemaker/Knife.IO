using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public Button backButton;
    public Button vibrateButton;
    public Button musicButton;

    public Sprite slide;
    public Sprite joystick;
    public Sprite vibrate;
    public Sprite nonVibrate;
    public Sprite music;
    public Sprite nonMusic;
    
    private void OnEnable()
    {
        ShowBox();
        backButton.onClick.AddListener(MoveBoxBack);
        
        vibrateButton.onClick.AddListener(() =>
        {
            if (vibrateButton.image.sprite == vibrate)
            {
                vibrateButton.image.sprite = nonVibrate;
                PlayerPrefs.SetInt(DataKey.Use_Vibrate, 0);
            }
            else if (vibrateButton.image.sprite == nonVibrate)
            {
                vibrateButton.image.sprite = vibrate;
                
                PlayerPrefs.SetInt(DataKey.Use_Vibrate, 1);
            }
        });
        
        musicButton.onClick.AddListener(() =>
        {
            if (musicButton.image.sprite == music)
            {
                musicButton.image.sprite = nonMusic;
                PlayerPrefs.SetInt(DataKey.Use_Music, 0);
                AudioManager.Instance.MuteMusic(true);
                AudioManager.Instance.MuteSFX(true);
            }
            else if (musicButton.image.sprite == nonMusic)
            {
                musicButton.image.sprite = music;
                PlayerPrefs.SetInt(DataKey.Use_Music, 1);
                AudioManager.Instance.MuteMusic(false);
                AudioManager.Instance.MuteSFX(false);
            }
        });
    }

    private void ShowBox()
    {
        LoadBoxData();
        MoveBox();
    }

    private void LoadBoxData()
    {
        if (DataKey.IsUseVibrate())
        {
            vibrateButton.image.sprite = vibrate;
        }
        else
        {
            vibrateButton.image.sprite = nonVibrate;
        }
        
        if (DataKey.IsUseMusic())
        {
            musicButton.image.sprite = music;
            AudioManager.Instance.MuteMusic(false);
            AudioManager.Instance.MuteSFX(false);
        }
        else
        {
            musicButton.image.sprite = nonMusic;
            AudioManager.Instance.MuteMusic(true);
            AudioManager.Instance.MuteSFX(true);
        }
    }
    
    private void MoveBox()
    {
        transform.position = startPos.position;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(endPos.position.y, 0.75f);
    }

    private void MoveBoxBack()
    {
        transform.DOLocalMoveY(startPos.localPosition.y, 0.75f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        
        vibrateButton.onClick.RemoveAllListeners(); 
        musicButton.onClick.RemoveAllListeners();
    }
}
