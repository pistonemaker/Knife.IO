using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer sr;
    public KnifeCollector knifeCollector;
    public Sprite rankSprite;
    public Sprite knifeSprite;
    public Player killer;
    public SpriteRenderer face;
    public SpriteRenderer playerDash;
    public Sprite dash;
    public ImageFace dataFace;
    public BattleRankUI rankUI;
    public TextMeshPro playerName;
    public TextMeshPro playerStateName;

    public int knifeAmount = 0;
    public int killAmount = 0;
    public int rank = 0;
    public int maxMinusSpeedRate = 6;
    public float speedMinus = 0.5f;
    private float initSpeed = 10f;
    public float curSpeed = 10f;

    private bool isSpeeding = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        knifeCollector = transform.Find("Knife Collector").GetComponent<KnifeCollector>();
        playerName = transform.Find("Name").GetComponent<TextMeshPro>();
        playerStateName = transform.Find("State").GetComponent<TextMeshPro>();
    }

    public void CreateKnifeInit(int initKnife)
    {
        knifeCollector.coll.radius = knifeCollector.initRadius;
        for (int i = 0; i < initKnife; i++)
        {
            var spawn = PoolingManager.Spawn(GameManager.Instance.knifePrefab, transform.position, Quaternion.identity);
            spawn.transform.SetParent(knifeCollector.transform);
            spawn.name = "Knife " + GameManager.Instance.stt++;
            var knife = spawn.GetComponent<Knife>();
            knife.canCollect = false;
            knife.knifeCollector = knifeCollector;
            knife.sr.sprite = knifeSprite;
            knifeCollector.knifes.Add(spawn);
            knifeCollector.SetAllKnifesAngle();
        }

        knifeAmount = initKnife;
        knifeCollector.AdjustColliderRadius();
    }

    public void ResetDataRespawn()
    {
        killAmount = 0;
        curSpeed = initSpeed;
        BalanceSpeed();
    }

    public void SetFaceAndKnife(ImageFace data, Sprite knife)
    {
        dataFace = data;
        face.sprite = dataFace.sprite1;
        knifeSprite = knife;
    }

    public void SetColorAndRankSprite(Sprite color, Sprite rankSpr)
    {
        sr.sprite = color;
        dash = color;
        rankUI.playerIcon.sprite = rankSpr;
    }

    public void FastSpeed()
    {
        StartCoroutine(BuffSpeed(4f, 7f));
    }

    private IEnumerator BuffSpeed(float rate, float time)
    {
        if (!isSpeeding)
        {
            isSpeeding = true;
            var curSpeed = this.curSpeed;
            this.curSpeed += rate;
            initSpeed += rate;
            StartCoroutine(SpawnPlayerDash(time));
            yield return new WaitForSeconds(time);
            isSpeeding = false;
            this.curSpeed = curSpeed;
            initSpeed -= rate;
        }
    }
    
    private IEnumerator SpawnPlayerDash(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            var dash = PoolingManager.Spawn(playerDash, transform.position, Quaternion.identity);
            dash.sprite = this.dash;

            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RotateKnifesStanding()
    {
        transform.DOKill();
        foreach (var knife in knifeCollector.knifes.ToList())
        {
            if (knife == null)
            {
                knifeCollector.knifes.Remove(knife);
                continue;
            }

            if (knife.transform.parent.name != knifeCollector.name)
            {
                knifeCollector.knifes.Remove(knife);
                Debug.Log(knife.name);
                continue;
            }

            var direction = (knife.transform.position - transform.position).normalized;
            var normalDirection = Vector3.Cross(direction, Vector3.forward).normalized;

            float angle = Mathf.Atan2(normalDirection.y, normalDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            knife.transform.DORotate(targetRotation.eulerAngles, 0.1f);
        }
    }

    public void RotateKnifesMoving()
    {
        transform.DOKill();
        foreach (var knife in knifeCollector.knifes.ToList())
        {
            if (knife == null)
            {
                knifeCollector.knifes.Remove(knife);
                continue;
            }

            if (knife.transform.parent.name != knifeCollector.name)
            {
                knifeCollector.knifes.Remove(knife);
                Debug.Log(knife.name);
                continue;
            }

            var direction = (knife.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            knife.transform.DORotate(targetRotation.eulerAngles, 0.1f);
        }
    }

    public void FixKnivesAngleAndDistance(GameObject knife, float angle)
    {
        if (knife != null)
        {
            float distance = knifeCollector.coll.radius / 2;

            Vector3 directionToPlayer = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 newPosition = transform.position + directionToPlayer.normalized * distance;

            knife.transform.position = newPosition;
            knife.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToPlayer);
        }
    }

    public void BalanceSpeed()
    {
        int minusCount = knifeAmount / 5;

        if (minusCount == 0)
        {
            return;
        }

        if (minusCount > maxMinusSpeedRate)
        {
            minusCount = maxMinusSpeedRate;
        }

        curSpeed = initSpeed - minusCount * speedMinus;
    }
}