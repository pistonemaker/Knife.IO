using UnityEngine;

public class MapAutoSetPos : MonoBehaviour
{
    private float space = 5.7f;
    private int sizeMap;
    public SpriteRenderer[] listMapSprite;

    private void Start()
    {
        // Lấy danh sách các miếng map trong GameObject cha
        listMapSprite = GetComponentsInChildren<SpriteRenderer>();
        

        // Kích thước của map
        sizeMap = (int)Mathf.Sqrt(listMapSprite.Length);
        int d = -sizeMap / 2;
        int c = sizeMap / 2;
        
        // Tính toán vị trí cho từng miếng map
        for (int i = 0; i < listMapSprite.Length; i++)
        {
            listMapSprite[i].transform.position = new Vector3(d++ * space, c * space, 0);
            listMapSprite[i].name = (i + 1).ToString();
            
            if (d > sizeMap / 2)
            {
                d = -sizeMap / 2;
                c--;
            }
        }
    }
}