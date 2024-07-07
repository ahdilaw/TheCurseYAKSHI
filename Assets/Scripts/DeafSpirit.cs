using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
 * Deaf Spirit's Mechanics:
 * Note:    The Player has four radii (colliders): (1) footsteps, (2) torchlight, (3) intimate, (4) body (non-trigger). See Player.cs.
 *  1.  PATROL MODE: Pre-spawned spirits are patrolers. They are always spawned and patrol between two flag points on the map, until they
 *      come into defend mode. Spirits for patrol purposes shall be provided with flagpoints in serialization, else they would turn to roaming.
 *  2.  ROAM MODE: Some randomly spawned spirits would also roam around the map to random locations, and then continue roaming around the map 
 *      further. In their way, if they come across the player, they turn to defend mode.
 *      reappearing after 3s gap. While in this state, Yakshi shall remain still (idle-animation), and wait for the player to come by.
 *  2.  DEFEND MODE (THROUGH SIGHT): If the deaf spirits see the player (from front 120d), they enter their defend mode, without delay. In 
 *      this mode, they shall pursue the player until they either, (a) comes out of the player's torchlight radius and losts the player, in which case
 *      they would still continue with their pursuit in the player's expected future direction for 1s, but after that, they would return to roaming/patrol,
 *      (b) if they move out of the certain radius (as serialized) of any of their flag points, they would immediately turn back to the nearest flag point, 
 *      and continue with patrol (this does not apply on roaming spirits).
 *  4.  ATTACK MODE: If spirits touches the body (collides), the Player is attacked and loses -1 health every second.
 *  *.  RECEIVE DAMAGE: (Not a Mode) The player can attack the spirits, and cause -1 health damage every 0.75 seconds. If they lose all their health, they become
 *      turn into dead bodies and their mechanics so apply (See Deadbody.cs).
 */

/* External Components Attachable to Yakshi:
 *  1.  Animator Class
 *  2.  The NavMesh2D Agent with the following const setup: Humanoid type with Obstacle Avoidance (r=0.15) and stopping distance in steer (0.2).
 *  3.  Rigidbody 2D with mass=3, linear_drag=9, and gravity_scale=0.
 *  4.  A CircleCollider2D (Triggered) and a non-trigger bodyfit CapsuleCollider2D.
 *  5.  An up bar for measuring their health.
 */

/* Events Listened to, or triggered:
 *  1.  (From HUD.cs) instructs to reduce the player health.
 */

public class DeafSpirit : MonoBehaviour
{

    /*DEAF SPIRIT MODES*/
    private const int _PATROLMODE = 1;
    private const int _ROAMMODE = 2;
    private const int _DEFENDMODE = 3;
    private const int _ATTACKMODE = 4;

    /*Constants*/
    private const int _FIRST = 1;
    private const int _SECOND = 2;
    private bool pooledObject = false;

    /*Mode Mechanics*/
    private float _AttackPauseTimer = 0f;
    private bool inflictPlayerAttack = false;
    private int _PursueMode = _ROAMMODE;
    private NavMeshAgent _Agent;
    private Animator animator;
    private int currentPatrolPoint = _FIRST;
    private float _PatrolPauseTimer = 0f;
    private Vector3 targetRoamPosition;
    private bool _InBatsAttack = false;
    private float _BatsAttackTimer = 0.5f;

    /*External References*/
    [SerializeField] private float health = 5f;
    [SerializeField] private HUD HUD;
    [SerializeField] private GameObject _ObjectPlayer;
    [SerializeField] private Transform firstFlagPoint;
    [SerializeField] private Transform secondFlagPoint;
    [SerializeField] private int _DefaultMode = _ROAMMODE;
    [SerializeField] private float _MaxDotProductInFront = 0.3f;
    [SerializeField] private float _MaxPatrolPointRad = 1.5f;
    [SerializeField] private float _RoamSearchRadius = 5f;

    /*Initialize through Object Pool*/
    public void Initialize()
    {
        //Overload to prevent errors in drag-drop 
    }

    public void Initialize(HUD hud, GameObject objectPlayer, Transform firstFlag, Transform secondFlag, int mode)
    {
        if (hud != null && objectPlayer != null && firstFlag != null && secondFlag != null)
        {
            HUD = hud;
            _ObjectPlayer = objectPlayer;
            firstFlagPoint = firstFlag;
            secondFlagPoint = secondFlag;
            _DefaultMode = mode;
            pooledObject = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
        if (_Agent != null)
        {
            _Agent.updateRotation = false;
            _Agent.updateUpAxis = false;
        }
        animator = GetComponent<Animator>();
        _PursueMode = _DefaultMode;
    }

    // Update is called once per frame
    void Update()
    {

        if (_InBatsAttack)
        {
            if (_BatsAttackTimer <= 0)
            {
                InflictBatsAttack();
                _BatsAttackTimer = 0.5f;
            }
            else _BatsAttackTimer -= Time.deltaTime;
        }

        /*MODE WISE ACTIONS*/
        switch (_PursueMode)
        {
            case _PATROLMODE:
                Debug.Log("patrol mode in update");
                BreakOtherModes(_PATROLMODE);
                PatrolFlagsAction();
                break;

            case _ROAMMODE:
                Debug.Log("roam mode in update");
                BreakOtherModes(_ROAMMODE);
                RoamAction();
                break;

            case _DEFENDMODE:
                Debug.Log("see mode in update");
                BreakOtherModes(_DEFENDMODE);
                PersuePlayerAction(true);
                break;

            case _ATTACKMODE:
                Debug.Log("attack mode in update");
                BreakOtherModes(_ATTACKMODE);
                //animator.SetTrigger("attack");
                if (inflictPlayerAttack) InflictAttackAction();
                PersuePlayerAction(false);
                break;

            default:
                Debug.Log("default mode in update as default");
                _PursueMode = _DefaultMode;
                break;

        }
    }

    /*Collision Triggers to activate modes (1)-(4)*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player--torchlight"))
        {
            Debug.Log("torchlight in trigger enter");
            if (TestInFrontOf())
            {
                Debug.Log("see mode in trigger enter");
                _PursueMode = _DEFENDMODE;
            }
        }
        else if (other.CompareTag("player--intimate"))
        {
            Debug.Log("intimate in trigger enter");
            Debug.Log("stalk mode in trigger enter");
            if (TestInFrontOf()) _PursueMode = _ATTACKMODE;
        }
        else if (other.CompareTag("Bats"))
        {
            _InBatsAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player--torchlight"))
        {
            Debug.Log("torchlight mode in trigger exit");
            Debug.Log("hear mode in trigger enter");
            _PursueMode = _DefaultMode;
        }
        else if (other.CompareTag("player--intimate"))
        {
            Debug.Log("intimate mode in trigger exit");
            _PursueMode = _DEFENDMODE;
        }
        else if (other.CompareTag("Bats"))
        {
            _InBatsAttack = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("player--body"))
        {
            Debug.Log("attack mode in collision enter");
            _PursueMode = _ATTACKMODE;
            inflictPlayerAttack = true;
        }
        if (AudioManager.instance != null && AudioManager.instance.IsPlayingBackgroundSFX("spirit") == false)
        {
            AudioManager.instance.PlayBackgroundSFX("spirit");
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("player--body"))
        {
            Debug.Log("attack mode in collision exit");
            _PursueMode = _DEFENDMODE;
            inflictPlayerAttack = false;
        }
    }

    /*Helper Functions Below:*/
    private bool TestInFrontOf()
    {
        if (_ObjectPlayer == null) return false;
        Vector3 targetDirection = (_ObjectPlayer.transform.position - transform.position).normalized;
        float dotAngle = Vector3.Dot(transform.up, targetDirection);
        Debug.Log("dot angle is: " + dotAngle);
        return (dotAngle > _MaxDotProductInFront);
    }

    private void BreakOtherModes(int mode)
    {
        if (mode != _ATTACKMODE) _AttackPauseTimer = 0;
        if (mode != _PATROLMODE) _PatrolPauseTimer = 0;

    }

    private void PatrolFlagsAction()
    {
        if (firstFlagPoint != null && secondFlagPoint != null)
        {
            if (_PatrolPauseTimer <= Mathf.Epsilon)
            {
                Vector3 flagPos;
                if (currentPatrolPoint == _FIRST) flagPos = firstFlagPoint.position; else flagPos = secondFlagPoint.position;
                Vector3 diffFlag = transform.position - flagPos;
                if (diffFlag.sqrMagnitude <= _MaxPatrolPointRad)
                {
                    //animator.SetTrigger("idle");
                    if (currentPatrolPoint == _FIRST) currentPatrolPoint = _SECOND; else currentPatrolPoint = _FIRST;
                    _PatrolPauseTimer = 1f;
                }
                else
                {
                    _Agent.SetDestination(flagPos);
                    Animate();
                }
            }
            else _PatrolPauseTimer -= Time.deltaTime;
        }
        else Animate();
    }

    private void RoamAction()
    {
        if (targetRoamPosition == null || _Agent.remainingDistance <= _Agent.stoppingDistance)
        {
            SetRoamPosition();
        }
        else
        {
            _Agent.SetDestination(targetRoamPosition);
            Animate();
        }
    }

    private void SetRoamPosition()
    {
        Vector3 randomPoint = Random.insideUnitSphere * _RoamSearchRadius;
        randomPoint += transform.position;
        if (CheckForNoObstacles(randomPoint))
        {
            targetRoamPosition = randomPoint;
            _Agent.SetDestination(targetRoamPosition);
            Animate();
        }
    }

    private bool CheckForNoObstacles(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void PersuePlayerAction(bool canOutrun)
    {
        if (canOutrun && _DefaultMode == _PATROLMODE && firstFlagPoint != null && secondFlagPoint != null)
        {
            float diffFirstTarget = (transform.position - firstFlagPoint.position).sqrMagnitude;
            float diffSecondTarget = (transform.position - secondFlagPoint.position).sqrMagnitude;
            if (diffFirstTarget > _MaxPatrolPointRad || diffSecondTarget > _MaxPatrolPointRad)
            {
                _DefaultMode = _PATROLMODE;
                return;
            }
        }
        _Agent.SetDestination(_ObjectPlayer.transform.position);
        Animate();
    }

    private void InflictAttackAction()
    {
        if (_AttackPauseTimer >= 0.5f)
        {
            //attack voice... huhh huhh
            Debug.Log("Spirtit Attacked the Player.");
            HUD.AttackFromSpirit();
            _AttackPauseTimer = 0;
        }
        else _AttackPauseTimer += Time.deltaTime;
    }

    private void Animate()
    {
        animator.SetBool("isMoving", _Agent.velocity.magnitude > 0);
        animator.SetFloat("moveX", _Agent.velocity.x);
        animator.SetFloat("moveY", _Agent.velocity.y);
    }

    private void InflictBatsAttack()
    {
        health -= 0.5f;
        if (health <= 0)
        {
            //animator.SetTrigger("dead");
            Debug.Log("Spirit is dead.");
            if (pooledObject)
            {
                gameObject.SetActive(false);
                //return to the pool
            }
            else Destroy(gameObject);
        }
    }

}
