using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScriptYakshi : MonoBehaviour
{
    [SerializeField] private float senseAndMoveSpeed = 0.6f;
    [SerializeField] private float hearAndMoveSpeed = 2.0f;
    [SerializeField] private float seeAndMoveSpeed = 2.0f;
    private float senseAngularTurn = 60.0f;
    private float hearAngularTurn = 120.0f;
    private float seeAngularTurn = 120.0f;
    private bool isInHearMode = false;
    private bool isInConfusionState = false;

    // AI Nav System
    [SerializeField] private GameObject targetPlayer;
    private Rigidbody2D rbPlayer;
    private NavMeshAgent selfAgent;
    private Vector3 targetMovePosition;
    [SerializeField] private float deltaFutureTime = 1.0f;

    // Player Sight
    private bool isPlayerInfront = false;
    private float playerFrontDotTS = 0.3f;

    // Combined Motion Behaviours
    private float weightTowardsPlayer = 1.0f;

    void Start()
    {
    }

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

    void Update()
    {
        if (selfAgent != null && targetPlayer != null && rbPlayer != null)
        {
            UpdateTargetMovePosition();
            SetTargetApproachRates();
            if (!isInConfusionState)
            {
                if (isPlayerInfront)
                {
                    if (!CheckForObstructions())
                    {
                        AttackPlayerInSight();
                    }
                    else
                    {
                        EnterConfusionState();
                    }
                }
                else
                {
                    RotateTowardsPlayer();
                    if (CheckForObstructions())
                    {
                        EnterConfusionState();
                    }
                    else
                    {
                        if (rbPlayer.velocity.Equals(Vector2.zero))
                        {
                            MoveTowardsPlayer();
                        }
                        else
                        {
                            MoveTowardsPlayerFuturePosition();
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("#this: Either the selfAgent or the targetPlayer is null.");
        }
    }

    private void AttackPlayerInSight()
    {
        // Add attack logic here
        Debug.Log("Yakshi is saying dialogs for attack.");
    }

    private void ConfusedState()
    {
        // Add confusion behavior logic here
        Debug.Log("#this: Yakshi is confused.");
    }

    private void SetTargetApproachRates()
    {
        if (isPlayerInfront)
        {
            selfAgent.speed = weightTowardsPlayer * seeAndMoveSpeed;
            selfAgent.angularSpeed = weightTowardsPlayer * seeAngularTurn;
        }
        else if (isInHearMode && !isPlayerInfront)
        {
            selfAgent.speed = weightTowardsPlayer * hearAndMoveSpeed;
            selfAgent.angularSpeed = weightTowardsPlayer * hearAngularTurn;
        }
        else
        {
            selfAgent.speed = weightTowardsPlayer * senseAndMoveSpeed;
            selfAgent.angularSpeed = weightTowardsPlayer * senseAngularTurn;
        }
    }

    private void MoveTowardsPlayerFuturePosition()
    {
        Vector3 linearVelocity = new Vector3(rbPlayer.velocity.x, rbPlayer.velocity.y, 0);
        Vector3 futurePosition = targetMovePosition + deltaFutureTime * Time.deltaTime * linearVelocity;
        selfAgent.SetDestination(futurePosition);
    }

    private void MoveTowardsPlayer()
    {
        selfAgent.SetDestination(targetMovePosition);
    }

    private void UpdateTargetMovePosition()
    {
        if (targetPlayer != null)
        {
            targetMovePosition = targetPlayer.transform.position;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log("#this: Yakshi is rotating towards the player.");
    }

    private bool CheckForObstructions()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (targetPlayer.transform.position - transform.position).normalized);
        if (hit.collider != null && hit.collider.gameObject != targetPlayer)
        {
            Debug.Log("#this: Obstruction found.");
            return true;
        }
        else
        {
            Debug.Log("#this: No obstruction.");
            return false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Footsteps"))
        {
            isInHearMode = true;
            Debug.Log("#this: Yakshi has heard the player.");
        }
        else if (other.CompareTag("--enemy"))
        {
            weightTowardsPlayer = 0.0f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Footsteps"))
        {
            isInHearMode = false;
            ExitConfusionState();
            Debug.Log("#this: Yakshi has lost the player's sound.");
        }
        else if (other.CompareTag("--enemy"))
        {
            weightTowardsPlayer = 1.0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Torchlight"))
        {
            UpdateIsPlayerInfront();
        }
    }

    private void UpdateIsPlayerInfront()
    {
        if (targetMovePosition != null)
        {
            Vector3 targetDirection = (targetMovePosition - transform.position).normalized;
            float dotAngle = Vector3.Dot(transform.up, targetDirection);
            if (dotAngle > playerFrontDotTS)
            {
                isPlayerInfront = true;
                Debug.Log("#this: Player is in sight.");
                ExitConfusionState();
            }
            else
            {
                isPlayerInfront = false;
                Debug.Log("#this: Player is behind.");
            }
        }
    }

    private void EnterConfusionState()
    {
        if (!isInConfusionState)
        {
            isInConfusionState = true;
            ConfusedState();
            Debug.Log("#this: Yakshi has entered confusion state.");
        }
    }

    private void ExitConfusionState()
    {
        if (isInConfusionState)
        {
            isInConfusionState = false;
            Debug.Log("#this: Yakshi has exited confusion state.");
        }
    }
}
