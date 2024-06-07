// Services/GemService.cs

using UnityEngine;

public class GemTestService :IGemTestService
{
    public GemModel GenerateCustomGemModel(GemType gemType, GemModel gemModel = null)
    {
        var gem = gemModel ?? new GemModel();

        gem.Type = gemType;
        gem.SpecialType = SpecialGemType.NotSpecial;
        gem.BasePoint = 1;
        gem.EffectStrength = 1;

        return gem;
    }
}