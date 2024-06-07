using System;

public class TileModel
{
    public event Action OnGemChanged;
    public int X { get; private set; }
    public int Y { get; private set; }

    public TileModel(int x, int y)
    {
        X = x;
        Y = y;
    }
}