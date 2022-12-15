using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject Key;

    private void Awake()
    {
        GameData.SelectedDifficuly = GameData.Difficuly.Normal;
    }

    // Start is called before the first frame update
    void Start()
    {
        //SpawnKey();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnKey()
    {
        float randomRange = Random.Range(-10.0f, 10.0f);
        Vector3 randomPosition = new Vector3(randomRange, 0, 0);

        var key = Instantiate(Key, randomPosition, Quaternion.identity);
        key.name = "Key";
    }
}