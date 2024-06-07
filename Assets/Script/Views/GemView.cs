using System;
using UnityEngine;
using Zenject;

public class GemView : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    private IMemoryPool _pool;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void PlayExplosionEffect()
    {
        // Implement explosion effect
    }

    public void PlayMoveAnimation(Vector3 targetPosition)
    {
        // Implement move animation
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
        gameObject.SetActive(true);
    }

    public void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _pool.Despawn(this);
    }


}