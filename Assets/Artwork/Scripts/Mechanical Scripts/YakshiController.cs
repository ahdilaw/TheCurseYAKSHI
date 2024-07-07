using System;
using Unity.VisualScripting;
using UnityEngine;

public class YakshiController : MonoBehaviour
{

    public float attractionForce = 12f;
    private Rigidbody2D rb;
    private Transform avatarTransform;
    private bool isInFootstepsRange = false;
    private bool isInTorchlightRange = false;
    private bool isInContact = false;
    private bool isInThunderstormRange = false;
    public LayerMask obstacleLayer;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        avatarTransform = GameObject.FindGameObjectWithTag("Avatar").transform;
    }

    void Update() {

        if (isInContact) {
            rb.velocity = Vector2.zero;
        } else if ((isInFootstepsRange && !isInThunderstormRange) || isInTorchlightRange) {
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

    private bool CanSeeAvatar() {
        Vector2 directionToAvatar = (avatarTransform.position - transform.position).normalized;
        float distanceToAvatar = Vector2.SqrMagnitude(transform.position - avatarTransform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToAvatar, distanceToAvatar, obstacleLayer);
        return hit.collider == null;  
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Avatar")) {
            isInContact = true;
            Debug.Log("Yakshi in contact with Avatar");
        }
        else if (other.CompareTag("Footsteps")) {
            isInFootstepsRange = true;
            Debug.Log("Yakshi entered the Footsteps range");
        } else if (other.CompareTag("Torchlight")) {
            isInTorchlightRange = true;
            Debug.Log("Yakshi entered the Torchlight range");
        } else if (other.CompareTag("Thunderstorm")) {
            isInThunderstormRange = true;
            Debug.Log("Yakshi entered the Thunderstorm range");
        }

    }

    void OnTriggerExit2D(Collider2D other) {
        
        if (other.CompareTag("Avatar")) {
            isInContact = false;
            Debug.Log("Yakshi left contact with Avatar");
        } else if (other.CompareTag("Footsteps")) {
            isInFootstepsRange = false;
            Debug.Log("Yakshi exited the Footsteps range");
        } else if (other.CompareTag("Torchlight")) {
            isInTorchlightRange = false;
            Debug.Log("Yakshi exited the Torchlight range");
        } else if (other.CompareTag("Thunderstorm")) {
            isInThunderstormRange = false;
            Debug.Log("Yakshi exited the Thunderstorm range");
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Bats"))
        {
            Vector2 direction = transform.position - collision.transform.position;
            direction.Normalize();
            rb.AddForce(direction * attractionForce, ForceMode2D.Impulse);
        }
    }

}
