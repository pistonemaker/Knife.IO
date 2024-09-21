using System.Collections;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public bool canCollect = true;
    public bool hasCollided = false;
    [SerializeField] private BoxCollider2D coll;
    [SerializeField] private Rigidbody2D rb;
    public KnifeCollector knifeCollector;

    public SpriteRenderer sr;
    private float force = -10f;
    private float rotateSpeed = 2000f;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.isKinematic = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeInHierarchy) return;

        if (other.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(other);
        }
        else if (other.gameObject.CompareTag("Bound"))
        {
            if (hasCollided && !canCollect)
            {
                StartCoroutine(CollideWithBound());
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
        else if (other.gameObject.CompareTag("Knife"))
        {
            HandleKnifeCollision(other);
        }
    }

    private void HandlePlayerCollision(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (knifeCollector != null && player != knifeCollector.player)
        {
            knifeCollector.player.killAmount++;
            player.killer = knifeCollector.player;
            GameManager.Instance.HandlePlayerDie(player);
            player.knifeCollector.ClearKnife();
            AudioManager.Instance.PlaySFX("Knife_Kill");
            EventDispatcher.Instance.PostEvent(EventID.Player_Die, player);

            if (player == GameManager.Instance.mainPlayer)
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlaySFX("Lose");
                EventDispatcher.Instance.PostEvent(EventID.Main_Player_Die, player);
            }
        }
    }

    private void HandleObstacleCollision(Collider2D other)
    {
        if (knifeCollector != null)
        {
            knifeCollector.SetAllKnifesAngle();

            if (knifeCollector.player != null)
            {
                if (knifeCollector.player == GameManager.Instance.mainPlayer)
                {
                    if (DataKey.IsUseVibrate())
                    {
                        Handheld.Vibrate();
                    }
                    AudioManager.Instance.PlaySFX("Knife_Hit", 0.5f);
                }
            }

            TriggerProcess(-transform.position, other.transform.position, false);
        }
    }

    private void HandleKnifeCollision(Collider2D other)
    {
        var collidedKnife = other.gameObject.GetComponent<Knife>();

        if (!hasCollided && !collidedKnife.hasCollided && knifeCollector != collidedKnife.knifeCollector && knifeCollector != null)
        {
            if (collidedKnife.knifeCollector != null)
            {
                hasCollided = true;
                collidedKnife.hasCollided = true;

                if (knifeCollector.player == GameManager.Instance.mainPlayer || collidedKnife.knifeCollector.player == GameManager.Instance.mainPlayer)
                {
                    if (DataKey.IsUseVibrate())
                    {
                        Handheld.Vibrate();
                    }

                    AudioManager.Instance.PlaySFX("Knife_Hit", 0.5f);
                }

                TriggerProcess(-transform.position, collidedKnife.transform.position, true);
                collidedKnife.TriggerProcess(transform.position, -collidedKnife.transform.position, true);
            }
            else
            {
                knifeCollector.HandleAddKnife(collidedKnife.gameObject);
            }
        }
    }

    private IEnumerator CollideWithBound()
    {
        rb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(0.5f);

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void TriggerProcess(Vector2 knife1Pos, Vector2 knife2Pos, bool canDestroyKnife)
    {
        if (!gameObject.activeInHierarchy) return;

        StartCoroutine(DisableColliderAndStopMovement(knife1Pos, knife2Pos, canDestroyKnife));

        knifeCollector.coll.radius -= 0.05f;

        knifeCollector.knifes.Remove(gameObject);
        GameManager.Instance.AddKnife(gameObject);
        knifeCollector.CheckKnife();
        knifeCollector.SetAllKnifesAngle();

        knifeCollector.player.knifeAmount--;

        if (knifeCollector.player == GameManager.Instance.mainPlayer)
        {
            GameManager.Instance.cameraSizeMoving -= 0.04f;
        }

        knifeCollector.player.BalanceSpeed();

        if (knifeCollector.player.knifeAmount < 0)
        {
            knifeCollector.player.knifeAmount = 0;
        }

        knifeCollector.player.rankUI.UpdateKnifeAmount();

        knifeCollector = null;
    }

    public IEnumerator DisableColliderAndStopMovement(Vector2 knife1Pos, Vector2 knife2Pos, bool canDestroyKnife)
    {
        if (!gameObject.activeInHierarchy) yield break;

        canCollect = false;
        rb.isKinematic = false;
        ApplyForce(knife1Pos, knife2Pos);

        yield return new WaitForSeconds(0.5f);

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(0.5f);
        canCollect = true;
        hasCollided = false;
        RandomDestroyKnife(canDestroyKnife);
    }

    public void ApplyForce(Vector2 forceDirection1, Vector2 forceDirection2)
    {
        float angle = Vector2.SignedAngle(forceDirection1, forceDirection2);

        Vector2 reverseDirection = Quaternion.Euler(0, 0, angle) * -forceDirection1;

        rb.AddForce(reverseDirection.normalized * force, ForceMode2D.Impulse);

        StartCoroutine(RotateKnife());
    }

    public IEnumerator RotateKnife()
    {
        float timer = 0f;
        while (timer < 0.5f)
        {
            transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void RandomDestroyKnife(bool canDestroyKnife)
    {
        if (!canDestroyKnife)
        {
            return;
        }

        int random = Random.Range(0, 5);

        if (random == 1 || random == 2 || random == 3)
        {
            GameManager.Instance.RemoveKnife(gameObject);
            PoolingManager.Despawn(gameObject);
        }
    }
}
