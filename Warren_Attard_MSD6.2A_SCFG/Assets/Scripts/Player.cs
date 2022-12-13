using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{ 

    public float timer = 3;
    private List<GameObject> tempItems = new List<GameObject>();
    public List<GameObject> Items = new List<GameObject>();
    public GameObject keyUI; 

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    private void OnTriggerEnter(Collider other)
    {
        //Add the item into a temporary List
        if(other.gameObject.tag == "Key")
        {
            tempItems.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Remove the item into a temporary List
        if (other.gameObject.tag == "Key")
        {
            tempItems.Remove(other.gameObject);
        }
    }
}
