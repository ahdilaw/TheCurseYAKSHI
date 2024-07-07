using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : MonoBehaviour
{
    public float moveSpeed = 2f; 
    public float changeDirectionInterval = 2f;  
    private Rigidbody2D rb;
    private Vector2 movement;
    private int health = 3;
    [SerializeField] private GameObject deadBodyPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        rb.velocity = movement * moveSpeed;
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Bats")) {
            health--;
            if (health <= 0) {
                TurnIntoDeadBody();
            }
        }
    }

    void TurnIntoDeadBody() {
        Instantiate(deadBodyPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
