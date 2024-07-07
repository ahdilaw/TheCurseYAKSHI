using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script restrains the other NPCs to come close and attack the player at the 
 * simultaneouly to pace the level of the game, and give the player a chance to 
 * outrun and escape the horrendous dangers of a simultaneous attack.
 */

public class ScriptAttackCollider : MonoBehaviour
{

    [SerializeField] private GameObject parent;
    [SerializeField] private float colliderRadiusNormal = 0.1f;
    [SerializeField] private float colliderRadiusSingleAttack = 1.4f;

    private CircleCollider2D parentCollider;

    private void Start()
    {
        if (parent != null && parent.GetComponent<CircleCollider2D>() != null)
        {
            parentCollider = parent.GetComponent<CircleCollider2D>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("--player"))
        {
            SetSingleEnemyAttack(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("--player"))
        {
            SetSingleEnemyAttack(false);
        }
    }

    private void SetSingleEnemyAttack(bool mode)
    {
        if (parentCollider != null)
        {
            if (mode) parentCollider.radius = colliderRadiusSingleAttack; else parentCollider.radius = colliderRadiusNormal;
        }
    }

}
