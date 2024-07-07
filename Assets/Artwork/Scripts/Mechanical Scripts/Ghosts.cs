using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ghosts : MonoBehaviour
{
    
    public float attractionForce = 1f;
    private Rigidbody2D rb;
    private Transform avatarTransform;
    public LayerMask obstacleLayer;
    private bool isInContact = false;
    private bool isInTorchlightRange = false;
    private int health = 4;
    [SerializeField] private GameObject deadBodyPrefab;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        avatarTransform = GameObject.FindGameObjectWithTag("Avatar").transform;
    }

    private bool CanSeeAvatar() {
        Vector2 directionToAvatar = (avatarTransform.position - transform.position).normalized;
        float distanceToAvatar = Vector2.Distance(transform.position, avatarTransform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToAvatar, distanceToAvatar, obstacleLayer);
        return hit.collider == null;  
    }

    void Update() {

        if (isInContact) {
            rb.velocity = Vector2.zero;
        } else if (isInTorchlightRange) {
            if (CanSeeAvatar()) {
                float force = attractionForce * UnityEngine.Random.Range(0.1f, 1f);
                Vector2 direction = (avatarTransform.position - transform.position).normalized;
                rb.velocity = direction * force;
            } else {
                rb.velocity = Vector2.zero;
            }
        } else {
            rb.velocity = Vector2.zero; 
        }

    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Avatar")) {
            isInContact = true;
            Debug.Log("Ghost in contact with Avatar");
        }
        else if (other.CompareTag("Torchlight")) {
            isInTorchlightRange = true;
            Debug.Log("Ghost entered the Torchlight range");
        }

    }

    void OnTriggerExit2D(Collider2D other) {
        
        if (other.CompareTag("Avatar")) {
            isInContact = false;
            Debug.Log("Ghost left contact with Avatar");
        } else if (other.CompareTag("Torchlight")) {
            isInTorchlightRange = false;
            Debug.Log("Ghost exited the Torchlight range");
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
