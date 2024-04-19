
using System;
using System.Collections;
using Script.Data;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelConfig[] levels;
    public int currentLevelIndex;

    private Match3 gameController;  // Reference to the main game controller
    private ScoreManager _scoreManager;

    private void Awake()
    {
        gameController = GetComponent<Match3>();
        _scoreManager = GetComponent<ScoreManager>();
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Length)
        {
            var config = levels[levelIndex];
            UpdateGlobalLevelConfig(config);
            gameController.SetupLevel();
            if (currentLevelIndex != levelIndex)
            {
                currentLevelIndex = levelIndex;
            }
        }
        else
        {
            Debug.LogError("Level index out of range");
        }
    }

    private void NextLevel()
    {
        if (currentLevelIndex + 1 < levels.Length)
        {
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("Last level reached. Restarting or ending game.");
            // Optionally restart the game or show end game content
        }
    }

    private void PreviousLevel()
    {
        if (currentLevelIndex - 1 >= 0)
        {
            LoadLevel(currentLevelIndex - 1);
        }
        else
        {
            Debug.Log("First level reached. Restarting or ending game.");
            // Optionally restart the game or show end game content
        }
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LevelScoreAcieved()
    {
        
    }

    public bool CheckCompletion()
    {
        //todo: add more completion type
        var targetScore = levels[currentLevelIndex].scoreToAchieve;
        if (_scoreManager.CurrentScore >= targetScore)
        {
            LevelComplete();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LevelComplete()
    {
        // show menu or go to next level
        Debug.Log("level completed");
        NextLevel();
    }
    
    // IEnumerator WaitAndLoadLevel() {
    //     while (gameController.IsGameLoopRunning) {
    //         Debug.Log($"gameController.IsGameLoopRunning is {gameController.IsGameLoopRunning}");
    //         yield return null; // Wait until the current game loop finishes
    //     }
    //     Debug.Log("level completed");
    //     NextLevel();
    // }

    private void UpdateGlobalLevelConfig(LevelConfig config)
    {
        ConfigurationManager.Instance.SetLevelConfig(config);
    }
    
    
}