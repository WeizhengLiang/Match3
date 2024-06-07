using UnityEngine;
using Zenject;

public class GenericPool<T> : MonoMemoryPool<Vector3, Quaternion, T> where T : MonoBehaviour, IPoolable<IMemoryPool>
{
    protected override void Reinitialize(Vector3 position, Quaternion rotation, T item)
    {
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.OnSpawned(this);
        // todo: add parent transform
    }
}