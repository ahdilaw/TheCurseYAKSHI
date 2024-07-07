using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Deadbody's Mechanics:
 *  1.  Died NPCs are converted to deadbodies, which are static and do not move. Deadbodies are not interactable, and 
 *  are only for visual representation of the dead NPCs.
 *  2.  THE DEAD CURSE: Upon touching a deadbody, the player is cursed, and the difficuly level of the game increases by 1, if
 *  not already maximum. (Implemented in Player.cs).
 *  3.  DEAD SMELL: The deadbodies emit a smell, which 
 *      (a) attracts the bats towards them, in which case the bats roam radius becomes small and they stick move around the deadbody,
 *          (Implemented in Bats.cs).
 *      (b) can cause the player to lose health if they stay near the deadbody for too long, at a rate of -0.5 health per second.
 */

/* External Components Attachable to Yakshi:
 *  1.  Animator Class
 *  2.  The NavMesh2D Agent with the following const setup: Humanoid type with Obstacle Avoidance (r=0.15) and stopping distance in steer (0.2).
 *  3.  Rigidbody 2D with mass=3, linear_drag=9, and gravity_scale=0.
 *  4.  A non-trigger bodyfit CapsuleCollider2D, and a trigger CircleCollider2D for smell.
 */

public class Deadbody : MonoBehaviour
{

    /*Mode Mechanics*/
    private bool isInSmellRange = false;
    private float smellTimer = 0f;

    /*External References*/
    [SerializeField] private HUD HUD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInSmellRange)
        {
            smellTimer += Time.deltaTime;
            if (smellTimer >= 1)
            {
                if (HUD != null) HUD.SmellFromDeadBody();
                smellTimer = 0;
            }
        }
    }

    /*Triggers*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player--body"))
        {
            isInSmellRange = true;
            smellTimer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player--body"))
        {
            isInSmellRange = false;
            smellTimer = 0;
        }
    }

}
