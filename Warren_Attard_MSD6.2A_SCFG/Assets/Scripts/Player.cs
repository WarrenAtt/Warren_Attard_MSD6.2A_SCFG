using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float health;
    private float maxHealth;
    private Image HealthBar;
    public GameObject keyUI;
    private TextMeshProUGUI scoreUI;
    private GameObject door, key;

    public float timer = 3;
    private List<GameObject> tempItems = new List<GameObject>();
    public List<GameObject> Items = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        health = GameData.Health;
        maxHealth = health;
        HealthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        scoreUI = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        key = GameObject.Find("Key");
        door = GameObject.Find("EscapeDoor");
    }

    // Update is called once per frame
    void Update()
    {
        //Start decreasing interaction timer
        if (Input.GetKey(KeyCode.E))
        {
            timer -= 1 * Time.deltaTime;
        }

        //Reset interaction timer if button is released
        if (Input.GetKeyUp(KeyCode.E))
        {
            timer = 3;
        }

        //Add the item to player's inventory if interaction timer is 0 and item is not already in inventory
        if(timer <= 0)
        {
            timer = 0;

            if(tempItems != null)
            {
                for(int i = 0; i < tempItems.Count; i++)
                {
                    if(tempItems[i] != Items.Contains(tempItems[i]))
                    {
                        Items.Add(tempItems[i]);

                        if (keyUI != null)
                        {
                            keyUI.SetActive(true);
                        }

                        tempItems[i].SetActive(false);
                    }
                }
            }
        }

        HealthBar.fillAmount = health / maxHealth;

        scoreUI.text = "Score: " + GameData.Score.ToString();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Add the item into a temporary List
        if(other.gameObject.name.Contains(key.gameObject.name))
        {
            tempItems.Add(other.gameObject);
        }

        if (other.gameObject.name == "EnemyHitbox")
        {
            switch (GameData.SelectedDifficuly)
            {
                case GameData.Difficuly.Easy:
                    health -= 10f;
                    break;
                case GameData.Difficuly.Normal:
                    health -= 30;
                    break;
                case GameData.Difficuly.Hard:
                    health -= 50;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Remove the item into a temporary List
        if (other.gameObject.name.Contains(key.gameObject.name))
        {
            tempItems.Remove(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Player will go to next level or complete game 
        if (collision.gameObject == door.gameObject && Items.Contains(key))
        {
            GameData.Health = health;

            if (SceneManager.GetActiveScene().name == "Level-01")
            {
                SceneManager.LoadScene("Level-02");
            }
            else if (SceneManager.GetActiveScene().name == "Level-02")
            {
                SceneManager.LoadScene("Level-03");
            }
            else if (SceneManager.GetActiveScene().name == "Level-03")
            {
                SceneManager.LoadScene("Victory");
            }
        }
    }
}
