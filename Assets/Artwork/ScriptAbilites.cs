using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Yakshi's Combined Movement Behaviours:
 * 1. At all times, Yakshi comes towards the player by sniffing the user's footsteps across the map.
 * 2. When Yakshi hears the player, it would start moving towards them fast.
 * 3. When Yakshi hits the torchlight from behind, it won't do anything. If it hits the torchlight form 
 * in front, Yakshi would cast a ray to check if the player has any kind of obstructions or not. If 
 * there is some obstruction, it would continue with the footstep (since the view is blocked), but if
 * there is no obstruction, Yakshi would move in faster and attack the player.
 * 4. If there is already some enemy attacking the player, Yakshi would stop and wait for the user to
 * outrun that before attacking itself in order to pace the difficulty of the game.
 */

/*
 * Currently Doing: Yakshi can kill the player on attack.
 */

public class ScriptAbilites : MonoBehaviour
{

    [SerializeField] private GameObject targetPlayer;
    private Rigidbody2D rbPlayer;
    private NavMeshAgent selfAgent;
    private Vector3 targetMovePosition;

    [SerializeField] private float senseSpeed = 0.2f;
    [SerializeField] private float senseAngle = 30f;
    [SerializeField] private float hearSpeed = 0.6f;
    [SerializeField] private float hearAngle = 60f;
    [SerializeField] private float seeSpeed = 2f;
    [SerializeField] private float seeAngle = 120f;
    [SerializeField] private float playerMaxCos = 0.3f;
    [SerializeField] private float futureTimeDelta = 1.0f;

    private bool canHearPlayer = false;
    private bool canSeePlayer = false;
    private bool checkPlayerInfront = false;
    private float paceMoveWeight = 1.0f;

    private void Awake()
    {
        selfAgent = GetComponent<NavMeshAgent>();
        if (selfAgent != null)
        {
            selfAgent.updateRotation = false;
            selfAgent.updateUpAxis = false;
        }
        if (targetPlayer != null && targetPlayer.GetComponent<Rigidbody2D>() != null)
        {
            rbPlayer = targetPlayer.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Footsteps"))
        {
            canHearPlayer = true;
            Debug.Log("#this: Yakshi has heard the player.");
        } 
        else if (other.CompareTag("Torchlight"))
        {
            checkPlayerInfront = true;
        }
        else if (other.CompareTag("--enemy"))
        {
            paceMoveWeight = 0.0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Footsteps"))
        {
            canHearPlayer = false;
            Debug.Log("#this: Yakshi has lost the player's sound.");
        }
        else if (other.CompareTag("Torchlight"))
        {
            checkPlayerInfront= false;
            canSeePlayer = false;
        }
        else if (other.CompareTag("--enemy"))
        {
            paceMoveWeight = 1.0f;
        }
    }

    void Update()
    {
        if (checkPlayerInfront)
        {
            if (CheckPlayerInFront()) canSeePlayer = true; else canSeePlayer = false;
        }
        if (selfAgent != null && targetMovePosition != null) {
            if (canSeePlayer)
            {
                if (!CheckObstructions()) MoveInSightedLocation(); else MoveInFootstepDirection();
            }
            else if (canHearPlayer)
            {
                MoveInFootstepDirection();
            }
            else
            {
                MoveInSensedDirection();
            }
        }
    }

    private void MoveInFootstepDirection()
    {
        SetTargetDirection();
        PaceMovements(hearSpeed, hearAngle);
        selfAgent.SetDestination(targetMovePosition);
    }

    private void SetTargetDirection()
    {
        if (targetPlayer != null) targetMovePosition = targetPlayer.transform.position;
    }

    private void PaceMovements(float speed, float angle)
    {
        selfAgent.speed = paceMoveWeight * speed;
        selfAgent.angularSpeed = paceMoveWeight * angle;
    }

    private void MoveInSightedLocation()
    {
        SetTargetDirection();
        PaceMovements(seeSpeed, seeAngle);
        if (rbPlayer.velocity != Vector2.zero)
        {
            Vector3 linearVelocity = new Vector3(rbPlayer.velocity.x, rbPlayer.velocity.y, 0);
            Vector3 futurePosition = targetMovePosition + futureTimeDelta * Time.deltaTime * linearVelocity;
            selfAgent.SetDestination(futurePosition);
        } else
        {
            selfAgent.SetDestination(targetMovePosition);
        }
    }

    private bool CheckObstructions()
    {
        int layerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (targetPlayer.transform.position - transform.position).normalized, 2.0f, layerMask);
        bool isObstructed = hit.collider != null;
        Debug.Log("is obstructed: " + isObstructed + " conditions: " + (isObstructed ? hit.collider.gameObject.name : "none"));
        return isObstructed;
    }

    private void MoveInSensedDirection()
    {
        SetTargetDirection();
        PaceMovements(senseSpeed, senseAngle);
        selfAgent.SetDestination(targetMovePosition);
    }

    private bool CheckPlayerInFront()
    {
        if (targetMovePosition == null) return false;
        Vector3 targetDirection = (targetMovePosition - transform.position).normalized;
        float dotAngle = Vector3.Dot(transform.up, targetDirection);
        Debug.Log("angle: " + dotAngle + " true/fasle: " + (dotAngle > playerMaxCos));
        return (dotAngle > playerMaxCos);
    }

}
