using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private List<GameObject> _waypoints;
    private List<GameObject> _npcs;
    private List<GameObject> _enemies;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 destination;
    private Vector3 playerPos;
    private bool isAlerted = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();
        _npcs = new List<GameObject>();
        _enemies = new List<GameObject>();

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            _npcs.Add(npc);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            _enemies.Add(enemy);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
        MoveToWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
        {
            _agent.isStopped = true;
            _animator.SetBool("isWalking", false);
            StartCoroutine(WaitTimer(2, MoveToWaypoint));
        }

        if(isAlerted == true)
        {
            MoveToWaypoint();

            if(_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
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

        if (isAlerted == false && _agent.remainingDistance <= 2f)
        {
            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        if (isAlerted == true)
        {
            destination = GameObject.Find("SafeArea").transform.position;
        }

        if(_agent.tag == "NPC")
        {
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(destination);
        }

        if (_agent.tag == "Enemy" && isAlerted == true)
        {
            _animator.SetBool("isTalking", false);
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(playerPos);
        }
    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.GetComponentInChildren<Light>().name == "EnemyHitbox" && other.gameObject.tag == "Player" && isAlerted == false)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].GetComponent<Enemy>().isAlerted = true;
            }

            for (int i = 0; i < _npcs.Count; i++)
            {
                _npcs[i].GetComponent<Enemy>().isAlerted = true;
            }

            MoveToWaypoint();

            print("Guards are Alerted!");
        }
    }
}