using System.Collections.Generic;

public class ScoreService : IScoreService
{
    public int CalculatePoint(List<MatchModel> matches)
    {
        var pointEarn = 0;
        foreach (var match in matches)
        {
            var basicPoints = CalculateBasicPoints(match);
            var comboPoints = CalculatePoints4Combo(match);
            pointEarn += basicPoints + comboPoints;
        }

        return pointEarn;
    }

    public int CalculateBasicPoints(MatchModel match)
    {
        var gemBasePoint = match.GemTypeBasePoint;
        return match.MatchCount * gemBasePoint;
    }

    public int CalculatePoints4Combo(MatchModel match)
    {
        var points4Combo = 0;
        var basePoint = match.GemTypeBasePoint;

        foreach (var specialEffect in match.SpecialEffects)
        {
            if (specialEffect.Value == 0) continue;

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