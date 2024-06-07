using UnityEngine;

public class GridModel
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float CellSize { get; private set; }
    public Vector3 Origin { get; private set; }

    public GridModel(int width, int height, float cellSize, Vector3 origin)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        Origin = origin;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return Origin + new Vector3(x, y) * CellSize;
    }
}