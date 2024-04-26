using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Core;
using TMPro;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    private int _pointsPerGem = 1;
    private int _bonusMultiplier = 2;
    public int CurrentScore { get; private set; }

    public void UpdateScore(List<Match> matches)
    {
        UpdateScoreValue(matches);
        UpdateScoreUI();
        AnimateScoreChange();
    }
    
    // add score
    public void UpdateScoreValue(List<Match> matches)
    {
        CurrentScore += CalculatePoint(matches);
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

    private int CalculatePoint(List<Match> matches)
    {
        var pointEarn = 0;
        foreach (var match in matches)
        {
            var basicPoints = CalculateBasicPoints(match);
            //var comboPoints = CalculatePoints4Combo(match);
            pointEarn += basicPoints;
        }

        return pointEarn;
    }

    private int CalculateBasicPoints(Match match)
    {
        
        return match.MatchCount;
    }

    private int CalculatePoints4Combo(Match match, int basicPoint)
    {
        // todo: finish the combo bonus calculation
        var points4Combo = 0;
        var basePoint = basicPoint;

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
