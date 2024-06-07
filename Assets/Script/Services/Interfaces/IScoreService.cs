using System.Collections.Generic;

public interface IScoreService
{
    int CalculatePoint(List<MatchModel> matches);
    int CalculateBasicPoints(MatchModel match);
    int CalculatePoints4Combo(MatchModel match);
}