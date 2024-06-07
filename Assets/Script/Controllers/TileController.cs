using UnityEngine;
using Zenject;

public class TileController : MonoBehaviour
{
    private TileModel _tileModel;
    private TileView tileView;
    private GemController gemController;
    private ITileService<GemController> _tileService;
    private GemController.Factory gemControllerFactory;
    private GenericPool<TileView> tilePool;

    [Inject]
    public void Construct(ITileService<GemController> service, GemController.Factory gemFactory, GenericPool<TileView> tilePool)
    {
        _tileService = service;
        gemControllerFactory = gemFactory;
        this.tilePool = tilePool;
    }

    public TileModel GetTileModel() => _tileModel;
    public TileView GetTileView() => tileView;

    public void Initialize(int x, int y, Vector3 worldPosition, LevelConfigModel config, Transform parent)
    {
        _tileModel = _tileService.CreateTileModel(x, y);
        tileView = _tileService.CreateTileView(tilePool, worldPosition, parent);

        gemController = gemControllerFactory.Create();
        if (config is CustomLevelConfigModel customConfig)
        {
            gemController.CustomInitialize(worldPosition, customConfig, x, y);
        }
        else
        {
            gemController.Initialize(worldPosition, config);
        }

        _tileModel.OnGemChanged += UpdateView;
        UpdateView();
    }

    public void SetGemController(GemController gemController)
    {
        this.gemController = gemController;
    }

    public GemController GetGemController() => gemController;

    public TileModel CreateTileModel(int x, int y)
    {
        return _tileService.CreateTileModel(x, y);
    }

    public TileView CreateTileView(Vector3 position, Transform parent)
    {
        return _tileService.CreateTileView(tilePool, position, parent);
    }

    private void UpdateView()
    {
        // if (gemController != null)
        // {
        //     gemController.GemView.transform.position = tileView.transform.position;
        // }
    }

    public class Factory : PlaceholderFactory<TileController>
    {
    }
}