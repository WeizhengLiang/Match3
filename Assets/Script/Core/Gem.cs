
using UnityEngine;

public class Gem : MonoBehaviour
{
    private GemType _type;

    public void SetType(GemType type)
    {
        this._type = type;
        GetComponent<SpriteRenderer>().sprite = type.Sprite;
    }

    public GemType GemType => _type;
}
