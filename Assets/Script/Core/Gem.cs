
using Script.Core;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private GemType _type;
    private SpecialGemType _specialType;

    public void SetType(GemType type, SpecialGemType specialType)
    {
        this._specialType = specialType;
        this._type = type;
        GetComponent<SpriteRenderer>().sprite = type.Sprite;
    }

    public GemType GemType => _type;
    public SpecialGemType SpecialGemType => _specialType;
}
