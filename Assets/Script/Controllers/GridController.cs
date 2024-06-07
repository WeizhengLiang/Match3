using UnityEngine;
using Zenject;

public class GridController : MonoBehaviour
{
    private TileController[,] _tileControllers;
    private IGridService<TileController> gridService;
    private GridModel gridModel;
    private GridView gridView;
    private TileController.Factory tileControllerFactory;
    private LevelConfigModel currentConfig;
    private CoordinateConverter _coordinateConverter;


    [Inject]
    public void Construct(IGridService<TileController> service, TileController.Factory tileFactory,
        CoordinateConverter coordinateConverter)
    {
        gridService = service;
        tileControllerFactory = tileFactory;
        _coordinateConverter = coordinateConverter;
    }

    public void Initialize(LevelConfigModel config)
    {
        gridModel = gridService.CreateGridModel(config.BoardWidth, config.BoardHeight, config.CellSize, config.Origin);
        gridView = gridService.CreateGridView(gameObject.GetComponent<GridView>(), config.Origin, transform); // todo: calculate center position for grid view
        
        if (_tileControllers == null)
        {
            _tileControllers = new TileController[config.BoardWidth, config.BoardHeight];
        }

        currentConfig = config;

        for (var x = 0; x < config.BoardWidth; x++)
        {
            for (var y = 0; y < config.BoardHeight; y++)
            {
                CreateBothGridAndGem(x, y);
            }
        }

        if (gridService == null)
        {
            Debug.LogError("Failed to initialize grid");
        }
        else
        {
            Debug.Log("Grid initialized successfully");
        }
    }

    public void ClearGrid()
    {
        if (gridService == null || _tileControllers == null) return;
        
        for (int x = 0; x < gridModel.Width; x++)
        {
            for (int y = 0; y < gridModel.Height; y++)
            {
                var gridObject = gridService.GetValue(x, y, gridModel,
                    _tileControllers);
                if (gridObject != null && gridObject.GetGemController() != null)
                {
                    var gemView = gridObject.GetGemController().GemView;
                    gemView.Dispose();
                    // gridService.SetValue(x, y, null, gridModel,
                    //     _tileControllers);
                }
            }
        }
    }

    public void CreateBothGridAndGem(int x, int y)
    {
        var tileController = tileControllerFactory.Create();
        tileController.Initialize(x, y, gridService.GetWorldPositionCenter(x, y, _coordinateConverter, gridModel),
            currentConfig, transform);
        _tileControllers[x, y] = tileController;
    }

    public void SetGem(int x, int y, GemController value)
    {
        var tile = gridService.GetValue(x, y, gridModel, _tileControllers);
        tile.SetGemController(value);
    }
    
    public GemController GetGem(int x, int y)
    {
        var tile = gridService.GetValue(x, y, gridModel, _tileControllers);
        return tile.GetGemController();
    }

    public void SetTile(int x, int y, TileController value)
    {
        var tile = value;
        gridService.SetValue(x, y, tile, gridModel, _tileControllers);
    }

    public TileController GetTile(int x, int y)
    {
        return gridService.GetValue(x, y, gridModel, _tileControllers);
    }

    public bool IsElementGridPositionValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < gridModel.Width && y < gridModel.Height;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        return gridService.GetXY(worldPosition, _coordinateConverter, gridModel);
    }

    public Vector3 GetWorldPositionCenter(int x, int y)
    {
        return gridService.GetWorldPositionCenter(x, y, _coordinateConverter, gridModel);
    }
}