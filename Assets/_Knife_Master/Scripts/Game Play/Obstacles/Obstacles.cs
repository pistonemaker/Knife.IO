using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private float moveSpeed = 0.3f; 
    private float rotateSpeed = 5f; 

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 randomDirection = Random.insideUnitCircle.normalized; 
        rb.velocity = randomDirection * moveSpeed; 
    }

    private void Update()
    {
        RotateObstacle();
    }

    private void RotateObstacle()
    {
        transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
    }
}