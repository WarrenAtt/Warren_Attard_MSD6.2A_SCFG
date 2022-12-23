using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public AudioSource BackgroundMusic;
    public GameObject EasyBtn, NormalBtn, HardBtn;
    

    private GameObject _player;
    private bool isPaused = false;
    

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        
    }

    // Start is called before the first frame update
    void Start()
    {
        BackgroundMusic.Play();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (_player != null && _player.GetComponent<Player>().health <= 0)
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name.Contains("Level"))
        {
            if (!isPaused)
            {
                StartCoroutine(PauseGame());
            }
            else
            {
                StartCoroutine(ResumeGame());
            }
            
        }
    }

    public void IncreaseScore(int ammount)
    {
        GameData.Score += ammount;
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

    public void EasyGameMode()
    {
        SelectDifficulty(GameData.Difficuly.Easy);

        StartGame();
    }

    public void NormalGameMode()
    {
        SelectDifficulty(GameData.Difficuly.Normal);

        StartGame();
    }

    public void HardGameMode()
    {
        SelectDifficulty(GameData.Difficuly.Hard);

        StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level-01");
    }

    public IEnumerator ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        print("Game Resumed! " + isPaused);
        yield return new WaitForSeconds(5);
    }

    private IEnumerator PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        print("Game Paused! " + isPaused);
        yield return new WaitForSeconds(5);
    }

    public void Victory()
    {
        SceneManager.LoadScene("Victory", LoadSceneMode.Single);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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