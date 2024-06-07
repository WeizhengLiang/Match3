using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _latestMatchScoreText;
    private IScoreService scoreService;
    private ConfigurationController configurationController;
    public int CurrentScore { get; private set; }
    public int LastMatchScore { get; private set; }

    [Inject]
    public void Construct(ConfigurationController configurationController, IScoreService scoreService)
    {
        this.configurationController = configurationController;
        this.scoreService = scoreService;
    }

    public void UpdateScore(List<MatchModel> matches)
    {
        UpdateScoreValue(matches);
        UpdateScoreUI();
        AnimateScoreChange();
    }

    public void UpdateScoreValue(List<MatchModel> matches)
    {
        LastMatchScore = scoreService.CalculatePoint(matches);
        CurrentScore += LastMatchScore;
    }

    public void UpdateScoreUI()
    {
        _scoreText.text = $"total score: {CurrentScore}/{configurationController.CurrentLevelConfig.ScoreToAchieve}";
        _latestMatchScoreText.text = $"last score: {LastMatchScore}";
    }

    public void AnimateScoreChange()
    {
        _scoreText.transform.DOPunchPosition(new Vector3(0, 10, 0), 0.5f);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        LastMatchScore = 0;
    }
}