using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    public struct Match
    {
        public List<Vector2Int> Positions { get; private set; }
        public Dictionary<SpecialGemType, int> SpecialEffects{ get; private set; }
        public int MatchCount => Positions.Count;

        public Match(List<Vector2Int> positions, Dictionary<SpecialGemType, int> specialEffects)
        {
            Positions = positions;
            SpecialEffects = specialEffects;
        }
        
        public Match(List<Vector2Int> positions)
        {
            Positions = positions;
            SpecialEffects = null;
        }
        
    }

}