
using Script.Core;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private GemType _type;
    private SpecialGemType _specialType;
    private int _basePoint;
    private int _effectStrength;

    public void SetType(GemType type, SpecialGemType specialType, int basePoint, int effectStrength)
    {
        this._specialType = specialType;
        this._type = type;
        this._basePoint = basePoint;
        this._effectStrength = effectStrength;
        GetComponent<SpriteRenderer>().sprite = type.Sprite;
    }

    public GemType GemType => _type;
    public SpecialGemType SpecialGemType => _specialType;
    public int BasePoint => _basePoint;
    public int EffectStrength => _effectStrength;
}
