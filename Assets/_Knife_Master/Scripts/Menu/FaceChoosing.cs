using UnityEngine;

public class FaceChoosing : Singleton<FaceChoosing>
{
    public Transform content;
    public FaceButton facePrefab;
    public FaceButton curChoosen;

    public void Init()
    {
        int count = MenuManager.Instance.imageData.faces.Count;
        
        for (int i = 0; i < count; i++)
        {
            var faceButton = PoolingManager.Spawn(facePrefab, transform.position, Quaternion.identity);
            faceButton.id = i;
            faceButton.transform.SetParent(content);
            faceButton.transform.localScale = Vector3.one;
            faceButton.faceImage.sprite = MenuManager.Instance.imageData.faces[i].sprite1;
            faceButton.data = MenuManager.Instance.imageData.faces[i];
            faceButton.hasClickedAfterUnlock = MenuManager.Instance.imageData.faces[i].hasClickedAfterUnlock;
            faceButton.unlockCondition = MenuManager.Instance.imageData.faces[i].unlockCondition;
        }
    }
}
