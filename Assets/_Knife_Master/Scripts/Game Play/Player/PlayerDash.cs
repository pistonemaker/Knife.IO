using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private float time = 0.25f;
    public SpriteRenderer sr;
    
    private void OnEnable()
    {
        StartCoroutine(InActive());
    }

    private IEnumerator InActive()
    {
        sr.DOFade(0.2f, 0.25f);
        yield return new WaitForSeconds(time);
        PoolingManager.Despawn(gameObject);
    }
}
