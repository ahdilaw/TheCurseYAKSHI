using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private ScreenFade screenFade;

    private PlayerControl playerControls;
    private bool isMoving;
    private bool is_I_Pressed = false;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerControls = new PlayerControl();
        rb = GetComponent<Rigidbody2D>();

        if (screenFade != null)
        {
            StartCoroutine(screenFade.FadeIn());
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    public void HandleUpdate() 
    {    
        PlayerInput();
    }
    private void FixedUpdate()
    {
        Move();
    }
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
    private void Move()
    {
        var moveTo = rb.position + movement * (movementSpeed * Time.fixedDeltaTime);

        if (!IsWalkable(moveTo))
        {

        }
        else {
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
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    void Interact()
    {
        var facingDirection = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPosition = transform.position + facingDirection;

        var collider = Physics2D.OverlapCircle(interactPosition, 0.2f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
}
