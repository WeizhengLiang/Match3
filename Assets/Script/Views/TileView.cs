using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class TileView : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
    private IMemoryPool _pool;
    
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void PlayFallAnimation(Vector3 targetPosition, float duration, TweenCallback onComplete)
    {
        transform.DOMove(targetPosition, duration).OnComplete(onComplete);
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