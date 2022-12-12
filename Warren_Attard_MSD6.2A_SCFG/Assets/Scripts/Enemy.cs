using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private List<GameObject> _waypoints;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        MoveToWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
        {
            _agent.isStopped = true;
            _animator.SetBool("isWalking", false);
            StartCoroutine(WaitTimer(2, MoveToWaypoint));
        }
    }

    private void MoveToWaypoint()
    {
        List<GameObject> tempWaypoints = new List<GameObject>();

        foreach (GameObject waypoint in _waypoints)
        {
            if (waypoint.transform.position != destination)
            {
                tempWaypoints.Add(waypoint);
            }
        }

        if (_agent.remainingDistance <= 2f)
        {

            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        _animator.SetBool("isWalking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);
    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
