using UnityEngine;

public class Rocket : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Knife"))
        {
            var knife = other.gameObject.GetComponent<Knife>();

            if (knife.knifeCollector)
            {
                if (knife.knifeCollector.player)
                {
                    knife.knifeCollector.player.FastSpeed();
                }
            }
            
            gameObject.SetActive(false);
            PoolingManager.Despawn(gameObject);
        }
    }
}
