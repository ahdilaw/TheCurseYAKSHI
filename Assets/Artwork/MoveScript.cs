using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveScript : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 target;
    [SerializeField] private GameObject targetGameObject;
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (targetGameObject == null)
        {
            Debug.Log("The game object is null");
        }
    }

    void Update()
    {
        SetTargetPosition();
        SetAgentPosition();
    }

    private void SetAgentPosition()
    {
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }

    private void SetTargetPosition()
    {
        /*if (Input.GetMouseButtonDown(0)) { 
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }*/
        target = targetGameObject.transform.position;
    }

}
