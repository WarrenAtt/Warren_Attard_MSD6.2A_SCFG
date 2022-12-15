using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    //Lists that will contain the AI's (enemies in the game)
    protected List<GameObject> AIs;
    protected List<GameObject> NPCs;
    protected List<GameObject> Enemies;

    //Determine the destination of the Agent
    private Vector3 destination;
    private Vector3 playerPos;

    //Agent states
    private float health;
    private bool isAlerted = false;
    private List<GameObject> _waypoints;
    private NavMeshAgent _agent;
    private Animator _animator;

    //Player's GameObject
    private GameObject _player;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();

        AIs = new List<GameObject>();
        NPCs = new List<GameObject>();
        Enemies = new List<GameObject>();

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            NPCs.Add(npc);
            AIs.Add(npc);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemies.Add(enemy);
            AIs.Add(enemy);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        //Setting the health for normal Agent
        if (gameObject.tag == "NPC")
        {
            health = 20;
        }

        //Setting the health for enemy Agent depending on the difficuly
        if (gameObject.tag == "Enemy")
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
        AIProperties(1);

        MoveToWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        //Retrieving Player's live position
        if(_player != null)
        {
            playerPos = _player.transform.position;
        }

        //Updating the Lists when enemy is destroyed
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

            //Destroy Agent
            Destroy(_agent.gameObject);
        }

        //Agent choosing next destination
        if (_agent.isOnNavMesh && _agent.remainingDistance <= 0.5f && !_agent.isStopped)
        {
            _agent.isStopped = true;
            _animator.SetBool("isWalking", false);
            StartCoroutine(WaitTimer(2, MoveToWaypoint));
        }

        //Normal Agent go to safe area, enemy Agent start attacking player
        if (isAlerted && _agent.isOnNavMesh)
        {
            MoveToWaypoint();

            if(_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
        }

        //Updating Agent's properties whilst running or alerted
        if(GameData.IsPlayerRunning || isAlerted)
        {
            AIProperties(1.2f);
        }
        else
        {
            AIProperties(1);
        }
    }

    //Handles the movement of the Agent
    private void MoveToWaypoint()
    {
        //Creating temporary waypoints
        List<GameObject> tempWaypoints = new List<GameObject>();

        //Searching all available waypoints in scene
        foreach (GameObject waypoint in _waypoints)
        {
            //Adding all the waypoints except the current waypoint to remove the possibily of the agent going on the same waypoint
            if (waypoint.transform.position != destination)
            {
                tempWaypoints.Add(waypoint);
            }
        }

        //Setting the desitination if Agents are not alerted 
        if (!isAlerted && _agent.remainingDistance <= 2f)
        {
            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        //Setting the destination if Agents are alerted
        if (isAlerted)
        {
            destination = GameObject.Find("SafeArea").transform.position;
        }

        //Setting normal Agent's Animation and assinging the desitination to Agent
        if(_agent.tag == "NPC" && _agent != null && _agent.isOnNavMesh)
        {
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(destination);
        }

        //Setting enemy Agent's Animation and assinging the desitination to Agent
        if (_agent.tag == "Enemy" && isAlerted && _agent.isOnNavMesh)
        {
            _animator.SetBool("isTalking", false);
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(playerPos);
        }
    }

    //Basic properties for Agent are set here.
    private void AIProperties(float multiplier)
    {
        //Searching all Agents in scene
        foreach (GameObject AI in AIs)
        {
            if(AI != null)
            {
                //Setting random speed to all agents
                AI.GetComponent<NavMeshAgent>().speed = UnityEngine.Random.Range(2.5f, 4.5f);

                //Changing detection properties depending on the difficulty
                switch (GameData.SelectedDifficuly)
                {
                    case GameData.Difficuly.Easy:
                        AI.GetComponentInChildren<Light>().spotAngle = 70 * multiplier;
                        AI.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 1f * multiplier;
                        break;
                    case GameData.Difficuly.Normal:
                        AI.GetComponentInChildren<Light>().spotAngle = 100 * multiplier;
                        AI.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 2f * multiplier;
                        break;
                    case GameData.Difficuly.Hard:
                        AI.GetComponentInChildren<Light>().spotAngle = 150 * multiplier;
                        AI.GetComponentInChildren<Light>().GetComponent<SphereCollider>().radius = 2.5f * multiplier;
                        break;
                }
            }
        }
    }

    //Simple timer method which allows the pause in the code, can start another method.
    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Alert Enemies on collision with Agent's hitbox
        if (gameObject.GetComponentInChildren<Light>() && other.gameObject.tag == "Player" && !isAlerted)
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
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Reduce Health on collision with bullet
        if(collision.gameObject.tag == "Bullet")
        {
            health -= 10f;
        }
    }
}