using UnityEngine;

[CreateAssetMenu(menuName = "Match3/GemType", fileName = "GemType")]
public class GemType : ScriptableObject
{
    public Sprite Sprite;
    public string ColorType;
}