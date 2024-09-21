using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> listSr;
    [SerializeField] private MapAutoSetPos mapAutoSetPos;

    public void SetMap(Sprite sprite)
    {
        mapAutoSetPos = GetComponentInChildren<MapAutoSetPos>();
        listSr = mapAutoSetPos.listMapSprite.ToList();
        for (int i = 0; i < listSr.Count; i++)
        {
            listSr[i].sprite = sprite;
        }
    }
}
