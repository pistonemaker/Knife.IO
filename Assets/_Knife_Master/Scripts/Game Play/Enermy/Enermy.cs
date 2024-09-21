using UnityEngine;

public class Enermy : MonoBehaviour
{
    private enum EnemyState
    {
        CollectKnife,
        Chase,
    }

    private Player player;
    [SerializeField] private Transform knifeTarget;
    [SerializeField] private Transform killTarget;
    private int knifeMinToAttack = 7;
    private int knifeMinToKillMainPlayer = 45;
    private float detectedRadius = 13f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask playerLayer;
    private EnemyState currentState;

    private void Start()
    {
        player = transform.parent.GetComponent<Player>();
        SetState(EnemyState.CollectKnife);
    }

    private void OnEnable()
    {
        killTarget = null;
        knifeTarget = null;
        SetState(EnemyState.CollectKnife);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.CollectKnife:
                CollectKnifeState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
        }

        player.RotateKnifesMoving();
    }

    private void CollectKnifeState()
    {
        player.playerStateName.text = "Collect Knife";
        killTarget = null;

        if (knifeTarget != null && GameManager.Instance.IsContainKnife(knifeTarget.gameObject))
        {
            MoveToTarget(knifeTarget.position, player.curSpeed);
        }
        else
        {
            if (!GameManager.Instance.HasKnife())
            {
                GameManager.Instance.SpawnRandomKnifes(5);
            }

            if (GameManager.Instance.GetRandomKnife().transform != null)
            {
                knifeTarget = GameManager.Instance.GetRandomKnife().transform;
                var knife = knifeTarget.GetComponent<Knife>();

                if (knife.knifeCollector != null || knife.canCollect == false)
                {
                    return;
                }
            }

            if (!CheckObstacleBetween(transform.position, knifeTarget.position) &&
                !CheckPlayerBetween(transform.position, knifeTarget.position))
            {
                return;
            }

            if (knifeTarget == null)
            {
                return;
            }

            MoveToTarget(knifeTarget.position, player.curSpeed);
        }

        CheckIfHasKillTarGet();
    }

    private void ChaseState()
    {
        player.playerStateName.text = "Chase";

        MoveToTarget(killTarget.position, player.curSpeed);

        if (!killTarget.gameObject.activeInHierarchy || player.knifeAmount <= knifeMinToAttack - 2)
        {
            SetState(EnemyState.CollectKnife);
        }
    }

    private void SetState(EnemyState newState)
    {
        currentState = newState;
    }

    private void CheckIfHasKillTarGet()
    {
        if (GameManager.Instance.mainPlayer.knifeAmount >= knifeMinToKillMainPlayer && player.knifeAmount >= 20)
        {
            killTarget = GameManager.Instance.mainPlayer.transform;
            SetState(EnemyState.Chase);
            return;
        }
        
        var hits = Physics2D.CircleCastAll(transform.position, detectedRadius, Vector2.zero);
        bool playerDetected = false;

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                var opponent = hit.collider.GetComponent<Player>();

                if (opponent != null && opponent != player)
                {
                    playerDetected = true;

                    if (CanKillTarget(opponent))
                    {
                        killTarget = opponent.transform;
                        SetState(EnemyState.Chase);
                    }

                    break;
                }
            }
        }

        if (!playerDetected)
        {
            SetState(EnemyState.CollectKnife);
        }
    }

    private void MoveToTarget(Vector3 targetPosition, float speed)
    {
        var position = transform.parent.position;
        var moveDirection = (targetPosition - position).normalized;

        RaycastHit2D obstacleHit = Physics2D.Raycast(position, moveDirection,
            speed * Time.deltaTime, obstacleLayer);

        if (obstacleHit.collider != null)
        {
            Vector2 newDirection = Vector2.Perpendicular(obstacleHit.normal).normalized;
            moveDirection = newDirection;
        }

        position += moveDirection * (speed * Time.deltaTime);
        transform.parent.position = position;
    }

    private bool CanKillTarget(Player target)
    {
        if (target == GameManager.Instance.mainPlayer)
        {
            if (player.knifeAmount > knifeMinToAttack - 2 && target.knifeAmount / 1.5f <= player.knifeAmount)
            {
                return true;
            }
        }
        else if (player.knifeAmount > knifeMinToAttack && target.knifeAmount <= player.knifeAmount)
        {
            return true;
        }

        return false;
    }

    private bool CheckObstacleBetween(Vector2 startP, Vector2 endP)
    {
        RaycastHit2D hit = Physics2D.Raycast(startP, endP - startP, Vector2.Distance(startP, endP), obstacleLayer);

        return hit.collider != null;
    }

    private bool CheckPlayerBetween(Vector2 startP, Vector2 endP)
    {
        RaycastHit2D hit = Physics2D.Raycast(startP, endP - startP,
            Vector2.Distance(startP, endP), playerLayer);

        if (hit.collider != null)
        {
            var playerHit = hit.collider.GetComponent<Player>();

            if (playerHit != null)
            {
                if (player.knifeAmount <= playerHit.knifeAmount)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (knifeTarget != null)
        {
            Gizmos.DrawLine(transform.position, knifeTarget.position);
        }

        if (killTarget != null)
        {
            Gizmos.DrawLine(transform.position, killTarget.position);
        }
    }
}