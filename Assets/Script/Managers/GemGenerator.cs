using System.Collections.Generic;
using System.Linq;
using Script.Core;
using UnityEngine;

public class GemGenerator : MonoBehaviour
{
    public Gem GenerateGems(Gem gem, LevelConfig config)
    {
        System.Random rand = new System.Random();
        
        GemType selectedGemType = ChooseWeightedRandom(config.availableGemTypeFrequencies, rand);
        int baseScore = rand.Next(config.GemBaseScoreRange.Min, config.GemBaseScoreRange.Max + 1);
        SpecialGemType selectedEffect = ChooseWeightedRandom(config.availableSpecialGemTypeFrequencies, rand);
        int effectStrength = rand.Next(config.SpecialEffectStrengthRange.Min, config.SpecialEffectStrengthRange.Max + 1);
        
        gem.SetType(selectedGemType, selectedEffect, baseScore, effectStrength);
        return gem;
    }

    private GemType ChooseWeightedRandom(GemTypeProbabilities[] probabilities, System.Random rand)
    {
        int totalWeight = 0;
        foreach (var item in probabilities)
        {
            totalWeight += item.weight;
        }

        int choice = rand.Next(totalWeight);
        int sum = 0;

        foreach (var item in probabilities)
        {
            sum += item.weight;
            if (choice < sum)
                return item.GemType;
        }

        return null;
    }
    
    private SpecialGemType ChooseWeightedRandom(SpecialGemTypeProbabilities[] probabilities, System.Random rand)
    {
        int totalWeight = 0;
        foreach (var item in probabilities)
        {
            totalWeight += item.weight;
        }

        int choice = rand.Next(totalWeight);
        int sum = 0;

        foreach (var item in probabilities)
        {
            sum += item.weight;
            if (choice < sum)
                return item.SpecialGemType;
        }

        return SpecialGemType.NotSpecial; // Assuming NotSpecial is a valid fallback
    }

    public Gem CreateGem(Gem gemPrefab, GridSystem2D<GridObject<Gem>> grid, int x, int y)
    {
        var gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        return gem;
    }
}