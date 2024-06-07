using UnityEngine;
using Zenject;

public class ConfigurationController : MonoBehaviour
{
    private IConfigService configService;

    [Inject]
    public void Construct(IConfigService configService)
    {
        this.configService = configService;
    }

    public void SetLevelConfig(LevelConfigModel config)
    {
        configService.SetLevelConfig(config);
    }

    public LevelConfigModel CurrentLevelConfig => configService.CurrentLevelConfig;
}