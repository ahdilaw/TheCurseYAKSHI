using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Players's Colectible Mechanics:
 * This script functions to collect objects like keys, and other items that the player can use to progress in the game.
 */

public class Collectibles : MonoBehaviour
{

    /*External References*/
    [SerializeField] private HUD HUD;
    [SerializeField] private ObjectPool pool;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*Triggers*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Deadbody"))
        {
            Debug.Log("The player touched a deadbody, and therefore, has been cursed.");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lives"))
        {
            Debug.Log("Lives collision detected.");
            CollectLives(other.gameObject);
        }
        else if (other.CompareTag("Batteries"))
        {
            Debug.Log("Batteries collision detected.");
            CollectBattery(other.gameObject);
        }
        else if (other.CompareTag("Keys"))
        {
            Debug.Log("Keys collision detected.");
            CollectKey(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

    }

    private void CollectLives(GameObject obj)
    {
        //live collected
        if (HUD != null) HUD.CollectedLife();
        Destroy(obj);
        //pool back to the object pool
    }

    private void CollectBattery(GameObject obj)
    {
        //live collected
        if (HUD != null) HUD.CollectedBattery();
        Destroy(obj);
        //pool back to the object pool
    }

    private void CollectKey(GameObject obj)
    {
        //live collected
        if (HUD != null) HUD.CollectedKey();
        Destroy(obj);
        //pool back to the object pool
    }

}
