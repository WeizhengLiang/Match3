public interface IConfigService
{
    LevelConfigModel CurrentLevelConfig { get; }
    void SetLevelConfig(LevelConfigModel config);
}