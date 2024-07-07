using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


/*
 * Yakshi's Mechanics:
 * Note:    The Player has four radii (colliders): (1) footsteps, (2) torchlight, (3) intimate, (4) body (non-trigger). See Player.cs. 
 *          For Yakshi, mode (1) through (4), and subparts therein, are mutually exclusive (one runs at a single time). Multiple modes can 
 *          stay active at a given time for any ONE from (1)-(4), and any MULTIPLE/ONE/NONE from (5)-(9).
 *  1.  IDLE/AMBUSH MODE: Randomly spawned (controlled by Spawner.cs) with a coroutine at all times for 5 seconds in select spawn points, 
 *      reappearing after 3s gap. While in this state, Yakshi shall remain still (idle-animation), and wait for the player to come by.
 *  2.  HEAR MODE: If Yakshi hears the some noise, after a reaction time of 1s, she can rapidly turn towards and move twoards the noise.
 *      This mode either applies if (a) Yakshi hears the player footsteps, or (b) Player enters Yakshi visibility from behind (240d from back). In
 *      case of (a) and (b), Yakshi shall turn and move towards the player until the player outrun their footsteps. This mode can also occur if
 *      (c) the player creaks out certain furniture or fights some enemy (causing a noise, a noise event shall trigger) AND Yakshi is in the 
 *      range of the noise so created. In such a case, Yakshi shall move towards the spot where noise is created.
 *  3.  SEE/STALK MODE: If Yakshi sees the player from in front (120d from front in torchlight), without the reaction time, she would enter into:
 *          (a) SEE MODE: Yakshi would remain silent and wait for the player to come closer (as as witch), and compute the player's velocity. If the 
 *          player recedes the movement (changes the direction), Yakshi would rotate towards the new player's front and teleport into their estimated
 *          future location. 
 *          (b) STALK MODE: If Yakshi enters the user's intimate radius, see mode turns into stalk mode after 0.5s in which Yakshi moves towards the 
 *          player directly and enters into attack mode, until otherwise evaded.
 *  4.  ATTACK MODE: If Yakshi touches the body (collides), the Player is attacked and is immediately died.
 */

/* External Components Attachable to Yakshi:
 *  1.  Animator Class
 *  2.  The NavMesh2D Agent with the following const setup: Humanoid type with Obstacle Avoidance (r=0.15).
 *  3.  Rigidbody 2D with mass=3, linear_drag=9, and gravity_scale=0.
 *  4.  A CircleCollider2D (Triggered) and a non-trigger bodyfit CapsuleCollider2D.
 */

/* Events Listened to, or triggered:
 *  1.  (From Player.cs) listens and gets the position on noise making.
 *  2.  (From HUD.cs) instructs to reduce the player health.
 */

public class Yakshi : MonoBehaviour

{

    /*YAKSHI MODES OF PERSUE*/
    private const int _IDLEMODE = 1;
    private const int _HEARMODE = 2;
    private const int _SEEMODE = 3;
    private const int _STALKMODE = 4;
    private const int _ATTACKMODE = 5;
    private int _PursueMode = _IDLEMODE;

    /*Mode Mechanics*/
    private float _HearPauseTimer = 0f;
    private bool _IsNoiseByProps = false;
    private NavMeshAgent _Agent;
    private Vector3 _HearPursuePosition;
    private float _StalkPauseTimer = 0f;
    private Vector3 _OriginSeePos;
    private Vector3 _RecedeSeePos;
    private Animator animator;

    /*Observed Events*/
    private Rigidbody2D _PlayerRigidbody;

    /*External References*/
    [SerializeField] private Player _Player;
    [SerializeField] private GameObject _ObjectPlayer;
    [SerializeField] private HUD HUD;
    [SerializeField] private float _MaxDotProductInFront = 0.3f;
    [SerializeField] private float _MaxNoiseDistance = 25f;
    [SerializeField] private float _TeleportPredictionTime = 1f;

    /*Initialize through Object Pool*/
    public void Initialize()
    {
        //Overload to prevent errors in drag-drop 
    }

    public void Initialize(Player player, GameObject objectPlayer, HUD hud)
    {
        if (player != null && objectPlayer != null && hud != null)
        {
            _Player = player;
            _ObjectPlayer = objectPlayer;
            HUD = hud;
        }
    }

    /*Regular Funcitons*/
    private void OnEnable()
    {
        if (_Player != null) _Player._NoiseMadeEvent += TriggerHearOnNoise;
    }

    void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
        if (_Agent != null)
        {
            _Agent.updateRotation = false;
            _Agent.updateUpAxis = false;
        }
        if (_ObjectPlayer != null && _ObjectPlayer.GetComponent<Rigidbody2D>()) _PlayerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (_Player != null) _Player._NoiseMadeEvent -= TriggerHearOnNoise;
    }

    // Update is called once per frame
    void Update()
    {

        /*MODE WISE ACTIONS*/
        switch (_PursueMode)
        {
            case _IDLEMODE:
                if (animator != null) animator.SetTrigger("idle");
                Debug.Log("idle mode in update");
                BreakOtherModes(_IDLEMODE);
                //idle mode trigger in animations and related SFX
                break;

            case _HEARMODE:
                Debug.Log("hear mode in update");
                BreakOtherModes(_HEARMODE);
                if (_HearPauseTimer >= 1 && _Agent != null) TurnAndMoveAction();
                else _HearPauseTimer += Time.deltaTime;
                break;

            case _SEEMODE:
                Debug.Log("see mode in update");
                BreakOtherModes(_SEEMODE);
                if (TestPlayerReceding()) TeleportAction(); else RemainSilentAction();
                break;

            case _STALKMODE:
                Debug.Log("stalk mode in update");
                BreakOtherModes(_STALKMODE);
                if (_StalkPauseTimer >= 0.5 && _Agent != null) StalkPlayerAction();
                else _StalkPauseTimer += Time.deltaTime;
                break;

            case _ATTACKMODE:
                Debug.Log("attack mode in update");
                BreakOtherModes(_ATTACKMODE);
                AttackAction();
                break;

            default:
                Debug.Log("idle mode in update as default");
                _PursueMode = _IDLEMODE;
                break;

        }
       
    }

    /*Collision Triggers to activate modes (1)-(4) and subparts therein*/
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("player--footsteps"))
        {
            Debug.Log("footsteps in trigger enter");
            Debug.Log("hear mode in trigger enter");
            _PursueMode = _HEARMODE;
        }
        else if (other.CompareTag("player--torchlight"))
        {
            Debug.Log("torchlight in trigger enter");
            if (TestInFrontOf())
            {
                Debug.Log("see mode in trigger enter");
                _PursueMode = _SEEMODE;
            }
        }
        else if (other.CompareTag("player--intimate"))
        {
            Debug.Log("intimate in trigger enter");
            Debug.Log("stalk mode in trigger enter");
            if (TestInFrontOf()) _PursueMode = _STALKMODE;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player--footsteps"))
        {
            Debug.Log("footsteps mode in trigger exit");
            Debug.Log("idle mode in trigger enter");
            _PursueMode = _IDLEMODE;
        }
        else if (other.CompareTag("player--torchlight"))
        {
            Debug.Log("torchlight mode in trigger exit");
            Debug.Log("hear mode in trigger enter");
            //_PursueMode = _HEARMODE;
            _PursueMode = _IDLEMODE;
        }
        else if (other.CompareTag("player--intimate"))
        {
            Debug.Log("intimate mode in trigger exit");
            if (TestInFrontOf())
            {
                Debug.Log("see mode in trigger exit");
                _PursueMode = _SEEMODE;
            }
            else
            {
                Debug.Log("hear mode in trigger exit");
                _PursueMode = _HEARMODE;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("player--body"))
        {
            Debug.Log("attack mode in collision enter");
            _PursueMode = _ATTACKMODE;
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
        if (mode != _HEARMODE) _HearPauseTimer = 0;
        if (mode != _HEARMODE) _IsNoiseByProps = false;
        if (mode != _STALKMODE) _StalkPauseTimer = 0;
    }

    private void TurnAndMoveAction()
    {
        if (!_IsNoiseByProps)
        {
            Vector3 playerPosition = _ObjectPlayer.transform.position;
            _Agent.SetDestination(playerPosition);
        }
        else
        {
            if (_HearPursuePosition != null) _Agent.SetDestination(_HearPursuePosition);
        }
        if (animator != null) animator.SetTrigger("walk");
        _HearPauseTimer = 0;
    }

    private void TriggerHearOnNoise(Vector3 position)
    {
        if (_PursueMode == _IDLEMODE || _PursueMode == _HEARMODE)
        {
            Debug.Log("Yakshi scipt triggered by Spacebar.");
            Vector3 distToPos = position - transform.position;
            bool res = distToPos.sqrMagnitude > _MaxNoiseDistance;
            Debug.Log("Trigger Res: " + res );
            if (distToPos.sqrMagnitude > _MaxNoiseDistance) return;
            _IsNoiseByProps = true;
            _HearPursuePosition = position;
            _PursueMode = _HEARMODE;
        }
    }

    private bool TestPlayerReceding()
    {

        if (_OriginSeePos == null)
        {
            _OriginSeePos = _ObjectPlayer.transform.position;
            return false;
        }

        if (_RecedeSeePos == null)
        {
            _RecedeSeePos = _ObjectPlayer.transform.position;
            return false;
        }

        Vector3 diff = (_OriginSeePos - _RecedeSeePos).normalized;
        float dotProduct = Vector3.Dot(transform.up, diff);

        Debug.Log("Recede Dot Product: " + dotProduct);
        _RecedeSeePos = _ObjectPlayer.transform.position;
        if (dotProduct < _MaxDotProductInFront)
        {
            Debug.Log("Player is not receding.");
            return false; 
        }
        else
        {
            Debug.Log("Player is receding.");
            return true; // Player is receding
        }
    }


    private void RemainSilentAction()
    {
        Vector3 directionPlayer = (_ObjectPlayer.transform.position - transform.position).normalized;
        Debug.Log("Remaining silent... player in front");
        _Agent.SetDestination(transform.position);
        if (animator != null) animator.SetTrigger("idle");
    }

    private void TeleportAction()
    {
        Vector2 currentPosition = _ObjectPlayer.transform.position;
        Vector3 futurePositon = new Vector3(currentPosition.x + _PlayerRigidbody.velocity.x * _TeleportPredictionTime, 
                                            currentPosition.y + _PlayerRigidbody.velocity.y * _TeleportPredictionTime, 0);
        if (CheckForNoObstacles(futurePositon))
        {
            _Agent.SetDestination(futurePositon);
            Debug.Log("teleporting to user's furture loc.. player in front receding");
            if (animator != null) animator.SetTrigger("run");
        }
        else
        {
            Debug.Log("Remaining silent... player in front not receding");
            _Agent.SetDestination(transform.position);
            if (animator != null) animator.SetTrigger("idle");
        }
    }

    private bool CheckForNoObstacles(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas);
    }

    private void StalkPlayerAction()
    {
        _Agent.SetDestination(_ObjectPlayer.transform.position);
        if (animator != null) animator.SetTrigger("run");
    }

    private void AttackAction()
    {
        if (animator != null) animator.SetTrigger("attack");
        //call to HUD instance to die the player, remove the object, and move onto the death screen.
        HUD.AttackFromYakshi();
    }

}
