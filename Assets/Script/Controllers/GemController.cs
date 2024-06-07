using UnityEngine;
using Zenject;

public class GemController : MonoBehaviour
{
    private GemModel gemModel;
    private GemView gemView;
    private IGemService gemService;
    private IGemTestService _testService;
    private GenericPool<GemView> gemPool;

    [Inject]
    public void Construct(IGemService service, GenericPool<GemView> gemPool, IGemTestService testService)
    {
        gemService = service;
        _testService = testService;
        this.gemPool = gemPool;
    }

    public void Initialize(Vector3 spawnPosition, LevelConfigModel config)
    {
        gemModel = gemService.GenerateGemModel(config);
        gemView = gemService.CreateGemView(gemPool, spawnPosition);
        gemModel.OnGemChanged += UpdateView;
        UpdateView();
    }
    
    public void CustomInitialize(Vector3 spawnPosition, CustomLevelConfigModel configModel, int x, int y)
    {
        gemModel = _testService.GenerateCustomGemModel(configModel.initialGems[x * 4 + y]);
        gemView = gemService.CreateGemView(gemPool, spawnPosition);
        gemModel.OnGemChanged += UpdateView;
        UpdateView();
    }

    private void UpdateView()
    {
        if (gemView == null)
        {
            Debug.LogWarning($"unable to update view because gemview is null");
            return;
        }
        gemView.SetSprite(gemModel.Type.Sprite);
    }

    public void DisposeGemView()
    {
        gemView.Dispose();
        gemView = null;
    }

    public void ExplodeGem()
    {
        
    }

    public GemModel GemModel => gemModel;
    public GemView GemView => gemView;
    public GemType GemType => gemModel.Type;
    public SpecialGemType SpecialGemType => gemModel.SpecialType;
    public int BasePoint => gemModel.BasePoint;
    public int EffectStrength => gemModel.EffectStrength;

    public void ReGenerateGemModel(LevelConfigModel config, GemModel gemModel = null)
    {
        this.gemModel = gemService.GenerateGemModel(config, gemModel);
    }

    public void ReGenerateGemView(Vector3 spawnPosition)
    {
        this.gemView = gemService.CreateGemView(gemPool, spawnPosition);
    }

    public class Factory : PlaceholderFactory<GemController>
    {
    }
}