using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Core;
using TMPro;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _latestMatchScoreText;
    private int _pointsPerGem = 1;
    private int _bonusMultiplier = 2;
    public int CurrentScore { get; private set; }
    public int LastMatchScore { get; private set; }

    public void UpdateScore(List<Match> matches)
    {
        UpdateScoreValue(matches);
        UpdateScoreUI();
        AnimateScoreChange();
    }
    
    // add score
    public void UpdateScoreValue(List<Match> matches)
    {
        LastMatchScore = CalculatePoint(matches);
        CurrentScore += LastMatchScore;
    }
    // update score UI
    public void UpdateScoreUI()
    {
        _scoreText.text = $"total score: {CurrentScore.ToString()}/{ConfigurationManager.Instance.CurrentLevelConfig.scoreToAchieve}";
        _latestMatchScoreText.text = $"last score: {LastMatchScore.ToString()}";
    }
    // animate UI changes
    public void AnimateScoreChange()
    {
        _scoreText.transform.DOPunchPosition(new Vector3(0, 10, 0), 0.5f);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        LastMatchScore = 0;
    }

    private int CalculatePoint(List<Match> matches)
    {
        var pointEarn = 0;
        foreach (var match in matches)
        {
            var basicPoints = CalculateBasicPoints(match);
            var comboPoints = CalculatePoints4Combo(match);
            pointEarn += basicPoints;
        }

        return pointEarn;
    }

    private int CalculateBasicPoints(Match match)
    {
        var gemBasePoint = match.GemTypeBasePoint;
        return match.MatchCount * gemBasePoint;
    }

    private int CalculatePoints4Combo(Match match)
    {
        // todo: finish the combo bonus calculation
        var points4Combo = 0;
        var basePoint = match.GemTypeBasePoint;

        foreach (var specialEffect in match.SpecialEffects)
        {
            if(specialEffect.Value == 0) continue;
            
            switch (specialEffect.Key)
            {
                case SpecialGemType.Additioner:
                    points4Combo += specialEffect.Value;
                    break;
                case SpecialGemType.Multiplier:
                    points4Combo += basePoint * specialEffect.Value;
                    break;
                case SpecialGemType.NotSpecial:
                    break;
            }
        }

        return points4Combo;
    }
}
