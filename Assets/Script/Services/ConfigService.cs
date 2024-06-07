public class ConfigService : IConfigService
{
    private LevelConfigModel currentLevelConfig;

    public LevelConfigModel CurrentLevelConfig => currentLevelConfig;

    public void SetLevelConfig(LevelConfigModel config)
    {
        currentLevelConfig = config;
    }
}