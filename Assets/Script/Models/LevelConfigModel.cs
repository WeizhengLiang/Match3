using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/LevelConfig", fileName = "LevelConfig")]
public class LevelConfigModel : ScriptableObject
{
    public int LevelNumber;
    public int ScoreToAchieve;
    public int BoardWidth;
    public int BoardHeight;
    public float CellSize;
    public Vector3 Origin;
    public float TimeLimit;
    public bool UseSpecialGems;
    public GemTypeProbabilities[] AvailableGemTypeFrequencies;
    public SpecialGemTypeProbabilities[] AvailableSpecialGemTypeFrequencies;
    public PointRange GemBaseScoreRange;
    public PointRange SpecialEffectStrengthRange;
}

[Serializable]
public class GemTypeProbabilities
{
    public GemType GemType;
    public int Weight;
}

[Serializable]
public class SpecialGemTypeProbabilities
{
    public SpecialGemType SpecialGemType;
    public int Weight;
}

[Serializable]
public class PointRange
{
    public int Min;
    public int Max;
}

public enum LevelGoalType
{
    CollectSomeGemType,
    AchieveSomeScoreWithSomeStepAmount,
}

public enum LevelLimitType
{
    LimitedTime,
    LimitedSteps,
    Obstacle
}

[Serializable]
public struct LevelGoal
{
    private LevelGoalType[] _levelGoals;
}