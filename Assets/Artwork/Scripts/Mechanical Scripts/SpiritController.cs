using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritController : MonoBehaviour
{
    
    public float attractionForce = 1f;
    private Rigidbody2D rb;
    private Transform avatarTransform;
    public LayerMask obstacleLayer;
    private bool isInContact = false;
    private bool isInFootstepsRange = false;
    private bool isInAttackMode = false;
    [SerializeField] private float stopDistance = 0.8f;
    private int health = 6;
    [SerializeField] private GameObject deadBodyPrefab;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        avatarTransform = GameObject.FindGameObjectWithTag("Avatar").transform;
        isInAttackMode = Random.value > 0.5f;
    }
    
    public bool GetAttackMode() {
        return isInAttackMode;
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
        } else if (isInFootstepsRange) {
            if (CanSeeAvatar()) {
                float distanceToAvatar = Vector2.Distance(transform.position, avatarTransform.position);
                if (distanceToAvatar > stopDistance) {
                    float force = attractionForce * UnityEngine.Random.Range(0.1f, 1f);
                    Vector2 direction = (avatarTransform.position - transform.position).normalized;
                    rb.velocity = direction * force;
                } else {
                    rb.velocity = Vector2.zero;
                }
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
            Debug.Log("Spirit in contact with Avatar");
        }
        else if (other.CompareTag("Footsteps")) {
            isInFootstepsRange = true;
            Debug.Log("Spirit entered the Footsteps range");
        }

    }

    void OnTriggerExit2D(Collider2D other) {
        
        if (other.CompareTag("Avatar")) {
            isInContact = false;
            Debug.Log("Spirit left contact with Avatar");
        } else if (other.CompareTag("Footsteps")) {
            isInFootstepsRange = false;
            Debug.Log("Spirit exited the Footsteps range");
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
