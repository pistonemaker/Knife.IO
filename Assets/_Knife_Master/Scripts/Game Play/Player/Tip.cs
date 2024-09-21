using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tip : Singleton<Tip>
{
    public int amountOfShowTipGames = 10;
    public List<Sprite> tipSprites;
    public Sprite moveTip;
    private Coroutine showTipRoutine;
    [SerializeField] private SpriteRenderer tipMove;
    [SerializeField] private SpriteRenderer tip;

    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener(EventID.Player_Kill_An_Enermy, HandleShowTip);
    }
    
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.Player_Kill_An_Enermy, HandleShowTip);
    }

    public void HandleShowTip(object param)
    {
        if (PlayerPrefs.GetInt(DataKey.Game_Count) > amountOfShowTipGames)
        {
            return;
        }

        showTipRoutine = StartCoroutine(ShowTip());
    }

    private IEnumerator ShowTip()
    {
        if (showTipRoutine != null)
        {
            StopCoroutine(showTipRoutine);
        }

        int random = Random.Range(0, tipSprites.Count);
        tip.sprite = tipSprites[random];
        tip.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        
        tip.gameObject.SetActive(false);
        showTipRoutine = null;
    }

    public void ShowMoveTip()
    {
        if (PlayerPrefs.GetInt(DataKey.Game_Count) > amountOfShowTipGames)
        {
            return;
        }
        
        tipMove.sprite = moveTip;
        tipMove.gameObject.SetActive(true);
    }

    public void HideMoveTip()
    {
        tipMove.gameObject.SetActive(false);
    }
}
