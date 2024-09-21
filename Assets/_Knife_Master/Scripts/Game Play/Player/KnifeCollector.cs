using System.Collections.Generic;
using UnityEngine;

public class KnifeCollector : MonoBehaviour
{
    public Player player;
    public CircleCollider2D coll;
    public float initRadius = 6f;
    public List<GameObject> knifes;
    [SerializeField] private float rotateSpeed;

    private void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        coll.radius = initRadius;
        player = transform.parent.GetComponent<Player>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, -1f) * (rotateSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Knife"))
        {
            var knifeGet = other.gameObject;
            var knifeCollector = knifeGet.GetComponentInParent<KnifeCollector>();
            
            if (knifeCollector == null)
            {
                HandleAddKnife(knifeGet);
            }
        }
        else if (other.gameObject.CompareTag("Rocket"))
        {
            other.gameObject.SetActive(false);
            player.FastSpeed();
        }
    }

    public void SetAllKnifesAngle()
    {
        float angleBetweenKnives = 360f / knifes.Count;

        for (int i = 0; i < knifes.Count; i++)
        {
            float angle = i * angleBetweenKnives;
            player.FixKnivesAngleAndDistance(knifes[i], angle);
        }
    }

    public void CheckKnife()
    {
        for (int i = knifes.Count - 1; i >= 0; i--)
        {
            if (knifes[i].transform.parent.parent.name != transform.parent.name || knifes[i] == null)
            {
                knifes.RemoveAt(i);
            }
        }
    }

    public void HandleAddKnife(GameObject knifeGet)
    {
        var knife = knifeGet.GetComponent<Knife>();
        
        if (!knifes.Contains(knifeGet) && knife.canCollect)
        {
            knife.canCollect = false;
            coll.radius += 0.05f;
            
            if (player == GameManager.Instance.mainPlayer)
            {
                GameManager.Instance.cameraSizeMoving += 0.04f;
            }
            
            GameManager.Instance.RemoveKnife(knifeGet);
            knifes.Add(knifeGet);
            knifeGet.transform.SetParent(transform);
            
            knife.sr.sprite = player.knifeSprite;
            player.knifeAmount++;
            player.BalanceSpeed();
            player.rankUI.UpdateKnifeAmount();
            SetAllKnifesAngle();
            
            knife.knifeCollector = this;
        }
    }

    public void ClearKnife()
    {
        for (int i = 0; i < knifes.Count; i++)
        {
            Destroy(knifes[i].gameObject);
            coll.radius -= 0.05f;
        }

        player.knifeAmount = 0;
        knifes.Clear();
        player.rankUI.UpdateKnifeAmount();
    }

    public void AdjustColliderRadius()
    {
        coll.radius += 0.05f * player.knifeAmount;
    }
}