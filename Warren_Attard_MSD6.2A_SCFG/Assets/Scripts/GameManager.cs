using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private GameObject _player;

    private void Awake()
    {
        SelectDifficulty(GameData.Difficuly.Normal);
    }

    // Start is called before the first frame update
    void Start()
    {
        //SpawnKey();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(_player == null)
        {
            GameOver();
        }
    }

    private void SelectDifficulty(GameData.Difficuly difficuly)
    {
        switch (difficuly)
        {
            case GameData.Difficuly.Easy:
                GameData.SelectedDifficuly = GameData.Difficuly.Easy;
                break;
            case GameData.Difficuly.Normal:
                GameData.SelectedDifficuly = GameData.Difficuly.Normal;
                break;
            case GameData.Difficuly.Hard:
                GameData.SelectedDifficuly = GameData.Difficuly.Hard;
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level-01");
        return; 
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
        return;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return;
    }
}