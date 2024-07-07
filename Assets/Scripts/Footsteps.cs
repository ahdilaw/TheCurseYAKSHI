using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    private CircleCollider2D footsteps;
    private bool isInThunderstorm = false;

    /*Constants*/
    private const float _NormalRadius = 3f;
    private const float _RatRadius = 5f;
    private const float _ThunderstormRadius = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<CircleCollider2D>()) footsteps = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*Triggers*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Thunderstorm"))
        {
            Debug.Log("Thunderstorm entered.");
            footsteps.radius = _ThunderstormRadius;
            isInThunderstorm = true;
        }
        else if (other.CompareTag("Rats"))
        {
            Debug.Log("Rats entered.");
            if (!isInThunderstorm)
            {
                footsteps.radius = _RatRadius;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Thunderstorm"))
        {
            Debug.Log("Thunderstorm exited.");
            footsteps.radius = _NormalRadius;
            isInThunderstorm = false;
        }
        else if (other.CompareTag("Rats"))
        {
            Debug.Log("Rats exited.");
            if (!isInThunderstorm)
            {
                footsteps.radius = _NormalRadius;
            }
        }
    }
}
