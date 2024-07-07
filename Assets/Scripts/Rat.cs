using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Rat's Mechanics:
 *  1.  AMPLIFY PLAYER FOOTSTEPS: (See Player.cs) Rats can amplify the noise of the footsteps of the player, possibly attracting the enemies.
 *      Implemented in the Player script.
 *  2.  MOVEMENT: Rats randomly roam around the level, reach any one point, wait for a random time, then continue with their journey.
 *  3.  RECEIVE DAMAGE: The player can attack rats, and they can die to turn into deadbodies.
 */

/* External Components Attachable to Yakshi:
 *  1.  Animator Class
 *  2.  The NavMesh2D Agent with the following const setup: Humanoid type with Obstacle Avoidance (r=0.15) and stopping distance in steer (0.2).
 *  3.  Rigidbody 2D with mass=3, linear_drag=9, and gravity_scale=0.
 *  4.  A non-trigger bodyfit CapsuleCollider2D.
 *  5.  An up bar for measuring their health.
 */


public class Rat : MonoBehaviour
{

    /*Mode Mechanics*/
    private float _MovePauseTimer = 0f;
    private Vector3 targetRoamPoint;
    private NavMeshAgent _Agent;
    private Animator animator;
    private bool pooledObject = false;
    private bool _InBatsAttack = false;
    private float _BatsAttackTimer = 0.5f;

    /*External References*/
    [SerializeField] private float health = 5f;
    [SerializeField] private float _RoamSearchRadius = 5f;

    public void Initialize() { }
    public void Initialize(int _) 
    { 
        pooledObject = true;
    }

    void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
        if (_Agent != null)
        {
            _Agent.updateRotation = false;
            _Agent.updateUpAxis = false;
        }
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        if (_InBatsAttack)
        {
            if (_BatsAttackTimer <= 0)
            {
                InflictBatsAttack();
                _BatsAttackTimer = 0.5f;
            }
            else _BatsAttackTimer -= Time.deltaTime;
        }

        if (_MovePauseTimer <= 0)
        {
            RoamAction();
        }
        else _MovePauseTimer -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bats"))
        {
            _InBatsAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bats"))
        {
            _InBatsAttack = false;
        }
    }


    /*Helper Functions*/
    private void RoamAction()
    {
        if (_Agent != null)
        {
            if (targetRoamPoint == null || _Agent.remainingDistance <= _Agent.stoppingDistance)
            {
                SetRoamPosition();
            }
            else
            {
                //_Agent.SetDestination(targetRoamPoint);
                Animate();
            }
        } 
        _MovePauseTimer = Random.Range(0.5f, 1f);
    }

    private void SetRoamPosition()
    {
        Vector3 randomPoint = Random.insideUnitSphere * _RoamSearchRadius;
        randomPoint += transform.position;
        if (CheckForNoObstacles(randomPoint))
        {
            targetRoamPoint = randomPoint;
            _Agent.SetDestination(targetRoamPoint);
            Animate();
        }
    }

    private bool CheckForNoObstacles(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void Animate()
    {
        animator.SetBool("isMoving", _Agent.velocity.magnitude > 0);
        animator.SetFloat("moveX", _Agent.velocity.x);
        animator.SetFloat("moveY", _Agent.velocity.y);
    }

    private void InflictBatsAttack()
    {
        health -= 0.5f;
        if (health <= 0)
        {
            //animator.SetTrigger("dead");
            Debug.Log("Spirit is dead.");
            if (pooledObject)
            {
                gameObject.SetActive(false);
                //return to the pool
            }
            else Destroy(gameObject);
        }
    }

}
