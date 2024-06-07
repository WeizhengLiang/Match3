using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridService<T> : IGridService<T>
{
    public event Action<int, int, T> OnValueChangeEvent;


    public GridModel CreateGridModel(int width, int height, float cellSize, Vector3 origin)
    {
        var model = new GridModel(width, height, cellSize, origin);
        return model;
    }

    public GridView CreateGridView(GridView prefab, Vector3 position, Transform parent)
    {
        var gridView = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        return gridView;
    }

    public void SetValue(Vector3 worldPosition, T value, CoordinateConverter coordinateConverter, GridModel gridModel,
        T[,] tileControllers)
    {
        Vector2Int pos = coordinateConverter.WorldToGrid(worldPosition, gridModel.CellSize, gridModel.Origin);
        SetValue(pos.x, pos.y, value, gridModel, tileControllers);
    }

    public void SetValue(int x, int y, T value, GridModel gridModel, T[,] tileControllers)
    {
        if (IsElementGridPositionValid(x, y, gridModel))
        {
            tileControllers[x, y] = value;
            OnValueChangeEvent?.Invoke(x, y, value);
        }
    }


    public T GetValue(Vector3 worldPosition, CoordinateConverter coordinateConverter, GridModel gridModel,
        T[,] tileControllers)
    {
        Vector2Int pos = GetXY(worldPosition, coordinateConverter, gridModel);
        return GetValue(pos.x, pos.y, gridModel, tileControllers);
    }

    public T GetValue(int x, int y, GridModel gridModel, T[,] tileControllers)
    {
        return IsElementGridPositionValid(x, y, gridModel) ? tileControllers[x, y] : default;
    }

    public bool IsElementGridPositionValid(int x, int y, GridModel gridModel)
    {
        if (x >= 0 && y >= 0 && x < gridModel.Width && y < gridModel.Height)
        {
            return true;
        }
        Debug.LogError($"position ({x}, {y}) is invalid");
        return false;
    }

    public Vector2Int GetXY(Vector3 worldPosition, CoordinateConverter coordinateConverter, GridModel gridModel)
    {
        return coordinateConverter.WorldToGrid(worldPosition, gridModel.CellSize, gridModel.Origin);
    }

    public Vector3 GetWorldPositionCenter(int x, int y, CoordinateConverter coordinateConverter, GridModel gridModel)
    {
        return coordinateConverter.GridToWorldCenter(x, y, gridModel.CellSize, gridModel.Origin);
    }
}