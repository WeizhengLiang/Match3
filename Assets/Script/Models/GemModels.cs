using System;

public class GemModel
{
    public event Action OnGemChanged;
    private GemType _type;
    private SpecialGemType _specialGemType;
    private int _basePoint;
    private int _effectStrength;

    public GemType Type
    {
        get => _type;
        set
        {
            _type = value;
            OnGemChanged?.Invoke();
        }
    }

    public SpecialGemType SpecialType
    {
        get => _specialGemType;
        set
        {
            _specialGemType = value;
            OnGemChanged?.Invoke();
        }
    }

    public int BasePoint
    {
        get => _basePoint;
        set
        {
            _basePoint = value;
            OnGemChanged?.Invoke();
        }
    }

    public int EffectStrength
    {
        get => _effectStrength;
        set
        {
            _effectStrength = value;
            OnGemChanged?.Invoke();
        }
    }
}