using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GemView gemViewPrefab;
    [SerializeField] private TileView tileViewPrefab;
    [SerializeField] private GemController gemControllerPrefab;
    [SerializeField] private TileController tileControllerPrefab;

    public override void InstallBindings()
    {
        Debug.Log("GameInstaller: InstallBindings called");
        Container.BindFactory<GemController, GemController.Factory>()
            .FromComponentInNewPrefab(gemControllerPrefab).AsSingle();
        Container.BindFactory<TileController, TileController.Factory>()
            .FromComponentInNewPrefab(tileControllerPrefab).AsSingle();

        Container.Bind<ITileService<GemController>>()
            .To<TileService<GemController>>()
            .AsSingle();
        Container.Bind<IGemService>()
            .To<GemService>()
            .AsSingle();
        Container.Bind<IGridService<TileController>>()
            .To<GridService<TileController>>()
            .AsSingle();
        Container.Bind<IConfigService>()
            .To<ConfigService>()
            .AsSingle();
        Container.Bind<IAudioService>()
            .To<AudioService>()
            .AsSingle();
        Container.Bind<IScoreService>()
            .To<ScoreService>()
            .AsSingle();
        Container.Bind<IMenuService>()
            .To<MenuService>()
            .AsSingle();

        Container.Bind<CoordinateConverter>().To<VerticalConverter>().AsSingle();

        Container.Bind<AudioController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<InputReader>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ConfigurationController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GridController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ScoreController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MenuController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        
        // Bind the GemPool
        Container.BindMemoryPool<GemView, GenericPool<GemView>>()
            .WithInitialSize(50)
            .FromComponentInNewPrefab(gemViewPrefab)
            .UnderTransformGroup("GemPool");

        // Bind the TilePool
        Container.BindMemoryPool<TileView, GenericPool<TileView>>()
            .WithInitialSize(50)
            .FromComponentInNewPrefab(tileViewPrefab)
            .UnderTransformGroup("TilePool");
        
        
        // test
        Container.Bind<IGemTestService>()
            .To<GemTestService>()
            .AsSingle();
     
    }
}