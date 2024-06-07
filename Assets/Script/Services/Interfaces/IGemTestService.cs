using UnityEngine;

public interface IGemTestService
{
    GemModel GenerateCustomGemModel(GemType gemType, GemModel gemModel = null);
}