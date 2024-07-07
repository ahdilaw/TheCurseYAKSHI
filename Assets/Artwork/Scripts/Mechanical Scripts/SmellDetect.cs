using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellDetect : MonoBehaviour
{
    
    private bool isInDeadRange = false;
    private HUDController hudController;
    private float timeSinceLastDecrease = 0f;
    [SerializeField] private float decreaseInterval = 2f;
    private bool isInGateRange = false;
    [SerializeField] private int minKeys = 3;

    void Start()
    {
        hudController = FindObjectOfType<HUDController>();
    }

    void Update()
    {

        if (isInDeadRange)
        {
            timeSinceLastDecrease += Time.deltaTime;
            if (timeSinceLastDecrease >= decreaseInterval)
            {
                hudController.DecrementScore();
                Debug.Log("Score Decremented");
                timeSinceLastDecrease = 0f;
            }
        }

        //Gate Check
        if (isInGateRange)
        {
            if (hudController.GetKeysCollected() >= minKeys)
            {
                Debug.Log("You have enough keys to open the gate");
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger Detected with " + other.gameObject.name);
        if (other.gameObject.name == "Smell")
        {
            isInDeadRange = true;
            Debug.Log("Avatar entered the DeadBody range");
        } else if (other.gameObject.name == "Gate")
        {
            isInGateRange = true;
            Debug.Log("Avatar entered the Gate range");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Trigger Exit Detected with " + other.gameObject.name);
        if (other.gameObject.name == "Smell")
        {
            isInDeadRange = false;
            Debug.Log("Avatar exited the DeadBody range");
        } else if (other.gameObject.name == "Gate")
        {
            isInGateRange = false;
            Debug.Log("Avatar exited the Gate range");
        }
    }
    
}
