using UnityEngine;

public interface ITileService<T>
{
    TileModel CreateTileModel(int x, int y);

    TileView CreateTileView(GenericPool<TileView> tilePool, Vector3 position, Transform parent);
    // void SetValue(TileController tile, T gem);
    // T GetValue(TileController tile);
}