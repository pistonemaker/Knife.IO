using UnityEngine;
using UnityEngine.UI;

public class KnifeButton : MonoBehaviour
{
    public bool isUnlocked;
    public bool hasClickedAfterUnlock;
    public int id;
    public int idColor;
    public Button button;
    public Image knifeImage;
    public Image border;
    public Image newIcon;
    public Image usedIcon;
    public Image progressLock;
    public Image progressUnlock;
    public UnlockCondition unlockCondition;

    private void OnEnable()
    {
        CheckIfUnlocked();
        SetBackGround();
        ShowProcess(Archievement.Instance.AmountProgress(unlockCondition.unlockType),
            unlockCondition.amountToUnlock);
        button.onClick.AddListener(OnButtonClicked);
    }

    private void SetBackGround()
    {
        if (id == 0)
        {
            button.image.sprite = ChooseBox.Instance.unlocked;
            hasClickedAfterUnlock = true;
            MenuManager.Instance.imageData.knifes[id].hasClickedAfterUnlock = true;
            progressLock.gameObject.SetActive(false);
        }
        else if (isUnlocked)
        {
            if (!hasClickedAfterUnlock)
            {
                newIcon.gameObject.SetActive(true);
            }
            
            button.image.sprite = ChooseBox.Instance.unlocked;
        }
        else
        {
            button.image.sprite = ChooseBox.Instance.locked;
        }
    }

    private void ShowProcess(float cur, float max)
    {
        float rate = cur / max;
        
        if(rate > 1)
        {
            rate = 1;
        }

        progressUnlock.fillAmount = rate;
    }

    private void OnButtonClicked()
    {
        knifeImage.sprite = KnifeChoosing.Instance.GetRandomSprite(id, this);

        if (isUnlocked)
        {
            if (KnifeChoosing.Instance.curChoosen != null)
            {
                KnifeChoosing.Instance.curChoosen.border.gameObject.SetActive(false);
                KnifeChoosing.Instance.curChoosen.usedIcon.gameObject.SetActive(false);
            }

            hasClickedAfterUnlock = true;
            MenuManager.Instance.imageData.knifes[id].hasClickedAfterUnlock = true;
            newIcon.gameObject.SetActive(false);
            KnifeChoosing.Instance.curChoosen = this;
            KnifeChoosing.Instance.curChoosen.border.gameObject.SetActive(true);
            KnifeChoosing.Instance.curChoosen.usedIcon.gameObject.SetActive(true);
            PlayerPrefs.SetInt(DataKey.ID_CurKnifeType, id);
            PlayerPrefs.SetInt(DataKey.ID_CurKnifeColor, idColor);
            MenuManager.Instance.imageData.curKnifeSprite = knifeImage.sprite;
        }

        ShowUnlockText();
        this.PostEvent(EventID.Knife_Choosed, knifeImage.sprite);
    }

    private void ShowUnlockText()
    {
        ChooseBox.Instance.unlockText.text =
            Archievement.Instance.FormatUnlockCondition(unlockCondition.unlockType, unlockCondition.amountToUnlock);
        ChooseBox.Instance.progressText.text =
            Archievement.Instance.AmountProgress(unlockCondition.unlockType) + "/" + unlockCondition.amountToUnlock;
        ShowUnlockProgress(Archievement.Instance.AmountProgress(unlockCondition.unlockType),
            unlockCondition.amountToUnlock);
    }

    private void ShowUnlockProgress(float cur, float max)
    {
        var unlockImage = ChooseBox.Instance.progressUnlock;
        var lockImage = ChooseBox.Instance.progressLock;
        float rate = cur / max;

        if (!lockImage.gameObject.activeInHierarchy)
        {
            lockImage.gameObject.SetActive(true);
        }

        if (rate > 1)
        {
            rate = 1;
        }

        unlockImage.fillAmount = rate;
    }

    public void CheckIfUnlocked()
    {
        if (Archievement.Instance.AmountProgress(unlockCondition.unlockType) >= unlockCondition.amountToUnlock)
        {
            MenuManager.Instance.imageData.knifes[id].isUnlock = true;
            progressLock.gameObject.SetActive(false);

            if (!isUnlocked)
            {
                isUnlocked = true;
                hasClickedAfterUnlock = false;
                MenuManager.Instance.imageData.knifes[id].hasClickedAfterUnlock = false;
            }
        }
        else
        {
            progressLock.gameObject.SetActive(true);
            MenuManager.Instance.imageData.knifes[id].isUnlock = false;
            isUnlocked = false;
            newIcon.gameObject.SetActive(false);
            hasClickedAfterUnlock = false;
            MenuManager.Instance.imageData.knifes[id].hasClickedAfterUnlock = false;
        }
    }
}