using UnityEngine;

public interface IGemService
{
    GemModel GenerateGemModel(LevelConfigModel config, GemModel gemModel = null);
    GemType ChooseWeightedRandom(GemTypeProbabilities[] probabilities, System.Random rand);
    SpecialGemType ChooseWeightedRandom(SpecialGemTypeProbabilities[] probabilities, System.Random rand);
    GemView CreateGemView(GenericPool<GemView> gemPool, Vector3 position);
    void BindGemModelView(GemModel gemModel, GemView gemView);
}