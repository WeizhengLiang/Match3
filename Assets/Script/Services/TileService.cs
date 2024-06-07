using UnityEngine;

public class TileService<T> : ITileService<T>
{

    public TileModel CreateTileModel(int x, int y)
    {
        return new TileModel(x, y);
    }

    public TileView CreateTileView(GenericPool<TileView> tilePool, Vector3 position, Transform parent)
    {
        var tileView = tilePool.Spawn(position, Quaternion.identity);
        return tileView;
    }

    // public void SetValue(TileController tile, T gem)
    // {
    //     tile.SetGemController(gem);
    // }
    //
    // public T GetValue(TileModel<T> tile)
    // {
    //     return tile.GetValue();
    // }
}