using System;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    public int CurrentScore { get; private set; }

    public void UpdateScore(int pointsToChange = 1)
    {
        UpdateScoreValue(pointsToChange);
        UpdateScoreUI();
        AnimateScoreChange();
    }
    
    // add score
    public void UpdateScoreValue(int pointsToChange)
    {
        CurrentScore += pointsToChange;
    }
    // update score UI
    public void UpdateScoreUI()
    {
        _scoreText.text = $"score: {CurrentScore.ToString()}/{ConfigurationManager.Instance.CurrentLevelConfig.scoreToAchieve}";
    }
    // animate UI changes
    public void AnimateScoreChange()
    {
        _scoreText.transform.DOPunchPosition(new Vector3(0, 10, 0), 0.5f);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
    }
}
