using System.Collections.Generic;
using UnityEngine;

namespace Script.Core
{
    public struct Match
    {
        public List<Vector2Int> Positions { get; private set; }
        public Dictionary<SpecialGemType, int> SpecialEffects{ get; private set; }
        public int MatchCount => Positions.Count;

        public readonly int GemTypeBasePoint;

        public Match(List<Vector2Int> positions, Dictionary<SpecialGemType, int> specialEffects, int gemTypeBasePoint)
        {
            Positions = positions;
            SpecialEffects = specialEffects;
            GemTypeBasePoint = gemTypeBasePoint;
        }
        

        public Gem GetFirstGem()
        {
            if (Positions.Count == 0)
            {
                Debug.LogWarning("the match is empty");
                return null;
            }
            var grid = Match3.Instance.grid;
            var gridObject = grid.GetValue(Positions[0].x, Positions[0].y);
            return gridObject.GetValue();
        }
        
    }

}