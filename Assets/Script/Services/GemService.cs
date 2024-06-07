// Services/GemService.cs

using UnityEngine;

public class GemService : IGemService
{
    public GemModel GenerateGemModel(LevelConfigModel config, GemModel gemModel = null)
    {
        var gem = gemModel ?? new GemModel();
        System.Random rand = new System.Random();

        GemType selectedGemType = ChooseWeightedRandom(config.AvailableGemTypeFrequencies, rand);
        int baseScore = rand.Next(config.GemBaseScoreRange.Min, config.GemBaseScoreRange.Max + 1);
        SpecialGemType selectedEffect = ChooseWeightedRandom(config.AvailableSpecialGemTypeFrequencies, rand);
        int effectStrength =
            rand.Next(config.SpecialEffectStrengthRange.Min, config.SpecialEffectStrengthRange.Max + 1);

        gem.Type = selectedGemType;
        gem.SpecialType = selectedEffect;
        gem.BasePoint = baseScore;
        gem.EffectStrength = effectStrength;

        return gem;
    }

    public GemType ChooseWeightedRandom(GemTypeProbabilities[] probabilities, System.Random rand)
    {
        int totalWeight = 0;
        foreach (var item in probabilities)
        {
            totalWeight += item.Weight;
        }

        int choice = rand.Next(totalWeight);
        int sum = 0;

        foreach (var item in probabilities)
        {
            sum += item.Weight;
            if (choice < sum)
                return item.GemType;
        }

        return null;
    }

    public SpecialGemType ChooseWeightedRandom(SpecialGemTypeProbabilities[] probabilities, System.Random rand)
    {
        int totalWeight = 0;
        foreach (var item in probabilities)
        {
            totalWeight += item.Weight;
        }

        int choice = rand.Next(totalWeight);
        int sum = 0;

        foreach (var item in probabilities)
        {
            sum += item.Weight;
            if (choice < sum)
                return item.SpecialGemType;
        }

        return SpecialGemType.NotSpecial; // Assuming NotSpecial is a valid fallback
    }

    public GemView CreateGemView(GenericPool<GemView> gemPool, Vector3 position)
    {
        var gemView = gemPool.Spawn(position, Quaternion.identity);
        return gemView;
    }

    public void BindGemModelView(GemModel gemModel, GemView gemView)
    {
        gemView.SetSprite(gemModel.Type.Sprite);
    }
}