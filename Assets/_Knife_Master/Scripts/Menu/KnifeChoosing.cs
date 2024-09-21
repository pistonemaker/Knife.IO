using UnityEngine;

public class KnifeChoosing : Singleton<KnifeChoosing>
{
    public Transform content;
    public KnifeButton knifePrefab;
    public KnifeButton curChoosen;

    public void Init()
    {
        int count = MenuManager.Instance.imageData.knifes.Count;
        
        for (int i = 0; i < count; i++)
        {
            var knifeButton = PoolingManager.Spawn(knifePrefab, transform.position, Quaternion.identity);
            knifeButton.id = i;
            knifeButton.transform.SetParent(content);
            knifeButton.transform.localScale = Vector3.one;
            knifeButton.knifeImage.sprite = GetRandomSprite(i, knifeButton);
            knifeButton.isUnlocked = MenuManager.Instance.imageData.knifes[i].isUnlock;
            knifeButton.isUnlocked = MenuManager.Instance.imageData.knifes[i].hasClickedAfterUnlock;
            knifeButton.unlockCondition = MenuManager.Instance.imageData.knifes[i].unlockCondition;
        }
    }

    public Sprite GetRandomSprite(int id, KnifeButton knifeButton)
    {
        int count = MenuManager.Instance.imageData.knifes[id].sprites.Count;
        int random = Random.Range(0, count);
        knifeButton.idColor = random;
        return MenuManager.Instance.imageData.knifes[id].sprites[random];
    }
}