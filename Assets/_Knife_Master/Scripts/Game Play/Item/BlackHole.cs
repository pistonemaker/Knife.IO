using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public SpriteRenderer sr;
    public CircleCollider2D coll;
    public Rigidbody2D rb;
    [SerializeField] private Transform knifeTrf;
    private float speed = 100f;
    private float moveSpeed = 0.5f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Vector2 randomDirection = Random.insideUnitCircle.normalized; 
        rb.velocity = randomDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var knife = other.gameObject.GetComponent<Knife>();
        if (other.gameObject.CompareTag("Knife") && knife.knifeCollector != null)
        {
            knife.canCollect = false;
            knife.knifeCollector.coll.radius -= 0.05f;

            if (knife.knifeCollector.player == GameManager.Instance.mainPlayer)
            {
                GameManager.Instance.cameraSizeMoving -= 0.04f;
            }
            
            coll.radius += 0.04f;
            sr.size = new Vector2(coll.radius * 2.5f, coll.radius * 2.5f);
            knife.knifeCollector.knifes.Remove(knife.gameObject);
            knife.knifeCollector.CheckKnife();
            knife.knifeCollector.SetAllKnifesAngle();
            
            knife.knifeCollector.player.knifeAmount--;
            knife.knifeCollector.player.BalanceSpeed();
        
            if (knife.knifeCollector.player.knifeAmount < 0)
            {
                knife.knifeCollector.player.knifeAmount = 0;
            }

            knife.knifeCollector.player.rankUI.UpdateKnifeAmount();
            knife.transform.SetParent(knifeTrf);
            knife.gameObject.SetActive(false);
            knife.knifeCollector = null;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * (speed * Time.deltaTime));
    }
}
