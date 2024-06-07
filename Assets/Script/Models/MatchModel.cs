using System.Collections.Generic;
using UnityEngine;

public class MatchModel
{
    public List<Vector2Int> Positions { get; private set; }
    public Dictionary<SpecialGemType, int> SpecialEffects { get; private set; }
    public int MatchCount => Positions.Count;
    public int GemTypeBasePoint { get; private set; }

    public MatchModel(List<Vector2Int> positions, Dictionary<SpecialGemType, int> specialEffects, int gemTypeBasePoint)
    {
        Positions = positions;
        SpecialEffects = specialEffects;
        GemTypeBasePoint = gemTypeBasePoint;
    }
}