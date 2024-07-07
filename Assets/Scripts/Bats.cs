using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
 * Bats's Mechanics:
 *  1.  ROAM FLIGHT: Bats randomly roam around the level, reach any one point, then continue with their journey, without delay.
 *  2.  IMPART DAMAGE: Bats can attack the player, and all other NPCs except Yakshi, on collision with them. They do not pursue the victim after 
 *  the attack, and rather move their way after imparting a -0.5 health damage to the victim on each trigger. (No collision with bats, since they fly.)
 */

/* External Components Attachable to Yakshi:
 *  1.  Animator Class
 *  2.  The NavMesh2D Agent with the following const setup: Humanoid type with Obstacle Avoidance (r=0.15) and stopping distance in steer (0.2).
 *  3.  Rigidbody 2D with mass=3, linear_drag=9, and gravity_scale=0.
 *  4.  A trigger bodyfit CapsuleCollider2D.
 */

public class Bats : MonoBehaviour
{

    /*Mode Mechanics*/
    private Vector3 targetRoamPoint;
    private NavMeshAgent _Agent;
    private Animator animator;

    /*External References*/
    [SerializeField] private float _RoamSearchRadius = 5f;

    public void Initialize() { }

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
        RoamAction();
    }

    /*Triggers*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Deadbody"))
        {
            Debug.Log("Deadbody trigger entered.");
            SetRoamSearchRadius(1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Deadbody"))
        {
            Debug.Log("Deadbody trigger exited.");
            SetRoamSearchRadius(5f);
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

    private void SetRoamSearchRadius(float radius)
    {
        _RoamSearchRadius = radius;
    }

}
