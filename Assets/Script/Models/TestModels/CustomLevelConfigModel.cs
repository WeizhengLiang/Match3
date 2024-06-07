using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3/CustomLevelConfig", fileName = "CustomLevelConfig")]
public class CustomLevelConfigModel : LevelConfigModel
{
    public GemType[] initialGems; // Array of GemTypes to place on the board
}