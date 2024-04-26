using Script.Core;

namespace Script.Data
{
    [System.Serializable]
    public class LevelConfig
    {
        public int levelNumber;
        public int scoreToAchieve;
        public int boardWidth;
        public int boardHeight;
        public float timeLimit;
        public bool useSpecialGems;
        public GemType[] availableGemTypes;  // Assuming GemType is already defined somewhere
        public SpecialGemType[] availableSpecialGemTypes;
    }

}