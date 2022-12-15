using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    protected List<GameObject> NPCs;
    protected List<GameObject> Enemies;

    private List<GameObject> _waypoints;
    private NavMeshAgent _agent;
    private Animator _animator;
    private GameObject _player;

    private Vector3 destination;
    private Vector3 playerPos;

    private float health;
    private bool isAlerted = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();
        NPCs = new List<GameObject>();
        Enemies = new List<GameObject>();

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            NPCs.Add(npc);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemies.Add(enemy);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (gameObject.tag == "NPC")
        {
            health = 20;
        }

        if(gameObject.tag == "Enemy")
        {
            switch (GameData.SelectedDifficuly)
            {
                case GameData.Difficuly.Easy:
                    health = 50;
                    break;
                case GameData.Difficuly.Normal:
                    health = 100;
                    break;
                case GameData.Difficuly.Hard:
                    health = 200;
                    break;
            }
        }

        foreach (GameObject enemy in Enemies)
        {
            enemy.GetComponent<NavMeshAgent>().speed = UnityEngine.Random.Range(2.5f, 4.5f);
        }

        MoveToWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(_player != null)
        {
            playerPos = _player.transform.position;
        }

        if (health <= 0)
        {
            List<GameObject> tempObj = new List<GameObject>();

            if (gameObject.tag == "NPC")
            {
                foreach(GameObject enemy in NPCs)
                {
                    if (enemy.GetHashCode() != gameObject.GetHashCode())
                    {
                        tempObj.Add(enemy);
                    }

                    enemy.GetComponent<AIController>().NPCs = tempObj;
                }
            }

            if (gameObject.tag == "Enemy")
            {
                foreach (GameObject enemy in Enemies)
                {
                    if (enemy.GetHashCode() != gameObject.GetHashCode())
                    {
                        tempObj.Add(enemy);
                    }

                    enemy.GetComponent<AIController>().Enemies = tempObj;
                }
            }

            Destroy(_agent.gameObject);
        }

        if (_agent.isOnNavMesh && _agent.remainingDistance <= 0.5f && !_agent.isStopped)
        {
            _agent.isStopped = true;
            _animator.SetBool("isWalking", false);
            StartCoroutine(WaitTimer(2, MoveToWaypoint));
        }
        

        if(isAlerted && _agent.isOnNavMesh)
        {
            MoveToWaypoint();

            if(_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
        }

        if(GameData.IsPlayerRunning || isAlerted)
        {
            foreach (GameObject enemy in Enemies)
            {
                if(enemy != null)
                {
                    enemy.GetComponentInChildren<Light>().spotAngle = 100;
                    enemy.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 2;
                }
            }

            foreach (GameObject enemy in NPCs)
            {
                if (enemy != null)
                {
                    enemy.GetComponentInChildren<Light>().spotAngle = 100;
                    enemy.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 2;
                }
            }
        }
        else
        {
            foreach (GameObject enemy in Enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponentInChildren<Light>().spotAngle = 70;
                    enemy.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 1;
                }
            }

            foreach (GameObject enemy in NPCs)
            {
                if (enemy != null)
                {
                    enemy.GetComponentInChildren<Light>().spotAngle = 70;
                    enemy.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 1;
                }
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

        foreach (GameObject enemy in NPCs)
        {
            enemy.GetComponent<NavMeshAgent>().speed = UnityEngine.Random.Range(2.5f, 4.5f);
        }


        if (!isAlerted && _agent.remainingDistance <= 2f)
        {
            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        if (isAlerted)
        {
            destination = GameObject.Find("SafeArea").transform.position;
        }

        if(_agent.tag == "NPC" && _agent != null && _agent.isOnNavMesh)
        {
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(destination);
        }

        if (_agent.tag == "Enemy" && isAlerted && _agent.isOnNavMesh)
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
        if (gameObject.GetComponentInChildren<Light>().name == "EnemyHitbox" && other.gameObject.tag == "Player" && !isAlerted)
        {
            foreach (GameObject enemy in NPCs)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<AIController>().isAlerted = true;
                }
            }

            foreach (GameObject enemy in Enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<AIController>().isAlerted = true;
                }
            }

            MoveToWaypoint();

            print("Guards are Alerted!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            health -= 10f;
        }
    }
}