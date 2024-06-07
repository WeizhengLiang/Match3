using UnityEngine;
using Zenject;

public class LevelController : MonoBehaviour
{
    public LevelConfigModel[] levels;
    public int currentLevelIndex;

    private GameManager gameController;
    private ScoreController scoreController;
    private ConfigurationController configurationController;

    [Inject]
    public void Construct(
        GameManager gameController,
        ScoreController scoreController,
        ConfigurationController configurationController
    )
    {
        this.gameController = gameController;
        this.scoreController = scoreController;
        this.configurationController = configurationController;
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
        var targetScore = levels[currentLevelIndex].ScoreToAchieve;
        if (scoreController.CurrentScore >= targetScore)
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

    private void UpdateGlobalLevelConfig(LevelConfigModel config)
    {
        configurationController.SetLevelConfig(config);
    }
}