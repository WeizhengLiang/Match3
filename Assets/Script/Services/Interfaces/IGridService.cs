using UnityEngine;

public interface IGridService<T>
{

    GridModel CreateGridModel(int width, int height, float cellSize, Vector3 origin);
    GridView CreateGridView(GridView prefab, Vector3 position, Transform parent);
    void SetValue(Vector3 worldPosition, T value, CoordinateConverter coordinateConverter, GridModel gridModel,
        T[,] tileControllers);

    void SetValue(int x, int y, T value, GridModel gridModel, T[,] tileControllers);

    T GetValue(Vector3 worldPosition, CoordinateConverter coordinateConverter, GridModel gridModel,
        T[,] tileControllers);

    T GetValue(int x, int y, GridModel gridModel, T[,] tileControllers);
    bool IsElementGridPositionValid(int x, int y, GridModel gridModel);
    Vector2Int GetXY(Vector3 worldPosition, CoordinateConverter coordinateConverter, GridModel gridModel);
    Vector3 GetWorldPositionCenter(int x, int y, CoordinateConverter coordinateConverter, GridModel gridModel);
}