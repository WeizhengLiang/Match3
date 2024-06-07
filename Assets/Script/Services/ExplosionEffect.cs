using UnityEngine;

public class ExplosionEffect : IGemEffect
{
    [SerializeField] private ParticleSystem explosionEffect;

    public void ActivateEffect(GemView gem)
    {
        // Assuming the effect is to play a particle system at the gem's position
        Object.Instantiate(explosionEffect, gem.transform.position, Quaternion.identity);
    }
}