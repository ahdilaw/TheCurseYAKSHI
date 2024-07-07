using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    public float moveSpeed = 2f; 
    public float changeDirectionInterval = 2f;  
    public float lifetime = 10f;
    private Rigidbody2D rb;
    private Vector2 movement;
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
        while (true) {
            lifetime -= changeDirectionInterval;
            if (lifetime <= 0) {
                Instantiate(deadBodyPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            yield return new WaitForSeconds(changeDirectionInterval);
        }
    }
    
}
