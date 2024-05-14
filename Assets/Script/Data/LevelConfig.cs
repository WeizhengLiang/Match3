using System;
using System.Collections.Generic;
using Script.Core;
using UnityEngine;


[CreateAssetMenu(menuName = "Match3/LevelConfig", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int levelNumber;
    public int scoreToAchieve;
    public int boardWidth;
    public int boardHeight;
    public float timeLimit;
    public bool useSpecialGems;
    public GemTypeProbabilities[] availableGemTypeFrequencies;  // Assuming GemType is already defined somewhere
    public SpecialGemTypeProbabilities[] availableSpecialGemTypeFrequencies;
    public PointRange GemBaseScoreRange;
    public PointRange SpecialEffectStrengthRange;
}

[Serializable]
public class GemTypeProbabilities
{
    public GemType GemType;
    public int weight;
}

[Serializable]
public class SpecialGemTypeProbabilities
{
    public SpecialGemType SpecialGemType;
    public int weight;
}

[Serializable]
public class PointRange
{
    public int Min;
    public int Max;
}