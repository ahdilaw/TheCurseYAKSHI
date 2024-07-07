using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Players's Mechanics:
 *  1.  Uses the arrow keys (AWSD or Arrow Keys) to move the player in 4 directions of motion.
 *  2.  Hold Right Shift while moving to sprint (Sprint does not work while in a thunderstorm).
 */

/*
 * Players's Dynamics:
 *  1.  RATS: When rats come into contact with the player's footsteps, they amplify the player's noise, possibly attracting the enemies.
 *  2.  THUNDERSTORM: When in thunderstorms, the player's footsteps are damped, and the player can not sprint.
 */

/* Events Listened to, or triggered:
 *  1.  (From Yakshi.cs) triggers the position on noise making.
 */

public class Player : MonoBehaviour
{

    /*Broadcasted Events*/
    public delegate void NoiseMade(Vector3 position);
    public event NoiseMade _NoiseMadeEvent;

    /*Player References*/
    private PlayerControl playerControls;
    private bool isMoving;
    private bool is_I_Pressed = false;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool canSprint = true;
    private GameObject attackingEnemy;
    //private float _AttackTimer = 0f;
    private bool isInBatAttack = false;
    private float _BatAttackTimer = 0f;
    private float movementSpeed;
    private float sprintTimer = 1f;

    /*External References*/
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private HUD HUD;
    [SerializeField] private ScreenFade screenFade;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerControls = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        movementSpeed = moveSpeed;
        if (screenFade != null) screenFade.FadeIn();
    }

    public void HandleUpdate()
    {

        if (isInBatAttack)
        {
            if (_BatAttackTimer <= 0)
            {
                InflictBatsAttack();
                _BatAttackTimer = 1f;
            }
            else _BatAttackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.RightShift) && canSprint)
        {
            if (HUD.getStamina() > 0)
            {
                movementSpeed = sprintSpeed;
                if (sprintTimer <= 0f)
                {
                    HUD.ReduceStamina();
                    sprintTimer = 1f;
                }
                else sprintTimer -= Time.deltaTime;
            }
        }
        else movementSpeed = moveSpeed;

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed.");
            TriggerNoise();
        }*/

        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /*Triggers*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Thunderstorm"))
        {
            Debug.Log("Thunderstorm collision detected.");
            canSprint = false;
        }
        else if (other.CompareTag("Bats"))
        {
            Debug.Log("Bats collision detected.");
            isInBatAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Thunderstorm"))
        {
            Debug.Log("Thunderstorm collision exited.");
            canSprint = true;
        }
        else if (other.CompareTag("Bats"))
        {
            Debug.Log("Bats collision detected.");
            isInBatAttack = false;
        }
    }

    /*New Input System Functions:*/
    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        bool is_I_held = playerControls.Interaction.Interact.ReadValue<float>() > 0.1f;

        if (is_I_held && !is_I_Pressed)
        {
            Interact();
            is_I_Pressed = true;
        }
        else if (!is_I_held)
        {
            is_I_Pressed = false;
        }
    }

    /*Helper Functions:*/
    private void TriggerNoise()
    {
        if (_NoiseMadeEvent != null)
        {
            Debug.Log("NoiseMade event triggered.");
            Vector3 currentPosition = transform.position;
            _NoiseMadeEvent(currentPosition);
        }
    }

    private void Move()
    {
        var moveTo = rb.position + movement * (movementSpeed * Time.fixedDeltaTime);
        if (IsWalkable(moveTo))
        {
            rb.MovePosition(moveTo);
            if (movement != Vector2.zero)
            {
                animator.SetFloat("moveX", movement.x);
                animator.SetFloat("moveY", movement.y);
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            animator.SetBool("isMoving", isMoving);
        }
    }

    private bool IsWalkable(Vector2 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer | interactableLayer) != null) return false;
        else return true;
    }

    void Interact()
    {
        var facingDirection = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPosition = transform.position + facingDirection;
        var collider = Physics2D.OverlapCircle(interactPosition, 0.2f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
            TriggerNoise();
        }
    }

    private void InflictBatsAttack()
    {
        //bad attacked
        if (HUD != null) HUD.AttackFromBat();
    }

}