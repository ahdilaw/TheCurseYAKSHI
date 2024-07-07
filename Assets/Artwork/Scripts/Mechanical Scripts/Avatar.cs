using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    private HUDController hudController;
    private Rigidbody2D rb;
    private CircleCollider2D footsteps;
    private bool isRunning = false;
    private bool isInGhostAttack = false;
    private float ghostAttacKTimer = 0f;
    private bool isInSpiritAttack = false;
    private float spiritAttackTimer = 0f;
    private bool spiritAttacker = false;
    private bool aroundRats = false;
    private float initialFootstepsRadius;
    private bool isInBatAttack = false;
    private float batAttackTimer = 0f;
    private Animator animator;
    void Start()
    {
        hudController = FindObjectOfType<HUDController>();
        rb = GetComponent<Rigidbody2D>();
        footsteps = GameObject.FindGameObjectWithTag("Footsteps").GetComponent<CircleCollider2D>();
        initialFootstepsRadius = footsteps.radius; 
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        /*
         * RUN/WALK MECHANICS IMPLEMENTED IN PLAYER CONTROLLER.CS SCRIPT
         */

        //Ghost Attack Mechanics
        if (isInGhostAttack) {
            ghostAttacKTimer += Time.deltaTime;
            if (ghostAttacKTimer >= 0.5f) {
                hudController.DecrementScore();
                Debug.Log("Ghost Attacked the Player");
                ghostAttacKTimer = 0f;
                //animator.SetTrigger("Hurt");
            }
        }

        //Spirit Attack Mechanics
        if (isInSpiritAttack && spiritAttacker) {
            spiritAttackTimer += Time.deltaTime;
            if (spiritAttackTimer >= 0.5f) {
                hudController.DecrementScore();
                Debug.Log("Spirit Attacked the Player");
                spiritAttackTimer = 0f;
                //animator.SetTrigger("Hurt");
            }
        }

        //Rats Louden Footsteps
        if (aroundRats && isRunning) {
            footsteps.radius = initialFootstepsRadius * 2.5f;
        } else if (aroundRats) {
            footsteps.radius = initialFootstepsRadius * 1.5f;
        } else if (isRunning) {
            footsteps.radius = initialFootstepsRadius * 2f;
        } else {
            footsteps.radius = initialFootstepsRadius;
        }

        //Bats Attack Mechanics
        if (isInBatAttack) {
            batAttackTimer += Time.deltaTime;
            if (batAttackTimer >= 0.5f) {
                hudController.DecrementScore();
                Debug.Log("Bat Attacked the Player");
                batAttackTimer = 0f;
                //animator.SetTrigger("Hurt");
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Yakshi"))
        {
            hudController.YakshiAttack();
            rb.velocity = Vector2.zero; 
            //animator.SetTrigger("Dead");
        }
        else if (collision.gameObject.CompareTag("Deadbody"))
        {
            hudController.StumbleOnDead();
            rb.velocity = Vector2.zero; 
            //animator.SetTrigger("Dead");
        }
        else if (collision.gameObject.CompareTag("Keys"))
        {
            hudController.CollectKey();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Battery"))
        {
            hudController.IncrementTorchlight();
            Destroy(collision.gameObject);
        } else if (collision.gameObject.CompareTag("Lives"))
        {
            if (hudController.incrementLives()) {
                Destroy(collision.gameObject);
            }
        } else if (collision.gameObject.CompareTag("Ghosts")) {
            isInGhostAttack = true;
        } else if (collision.gameObject.CompareTag("Spirits")) {
            isInSpiritAttack = true;
            spiritAttacker = collision.gameObject.GetComponent<SpiritController>().GetAttackMode();
        } else if (collision.gameObject.CompareTag("Bats")) {
            isInBatAttack = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision Ended with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Ghosts")) {
            isInGhostAttack = false;
        } else if (collision.gameObject.CompareTag("Spirits")) {
            isInSpiritAttack = false;
            spiritAttacker = false;
        } else if (collision.gameObject.CompareTag("Bats")) {
            isInBatAttack = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rats")) {
            aroundRats = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
       if (other.CompareTag("Rats")) {
            aroundRats = false;
        }
    }

}
