using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchCollider : MonoBehaviour
{
    
    private float remainingTime = 240f;
    private HUDController hudController;
    private float timer = 0f;
    private CircleCollider2D circleCollider;
    [SerializeField] private float shrinkRate = 0.02f;
    private bool isInThunderstormRange = false;

    void Start() {
        hudController = FindObjectOfType<HUDController>();
        remainingTime = hudController.GetTorchlight();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update() {

        if (remainingTime > 0) {
            timer += Time.deltaTime;
            if (timer >= 1f) {
                hudController.DecrementTorchlight();
                timer = 0f;
                remainingTime = hudController.GetTorchlight();                
                if (remainingTime <= 60f){
                    Debug.Log("Shrinking radius");
                    circleCollider.radius -= shrinkRate;
                    if (circleCollider.radius < 0.1f) {
                        Destroy(gameObject);
                    }
                } else if (remainingTime <= 0) {
                    Destroy(gameObject);
                } else {
                    circleCollider.radius = 2f;
                }
            }
        }

        if (isInThunderstormRange) {
            //2x the decrement rate
            timer += Time.deltaTime;
        }

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Thunderstorm")) {
            isInThunderstormRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Thunderstorm")) {
            isInThunderstormRange = false;
        }
    }


}
