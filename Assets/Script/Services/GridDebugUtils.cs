using TMPro;
using UnityEngine;

public static class GridDebugUtils
{
    public static void DrawDebugLines<T>(GridModel model, CoordinateConverter coordinateConverter)
    {
        const float duration = 100f;
        var parent = new GameObject("Debugging");

        for (int x = 0; x < model.Width; x++)
        {
            for (int y = 0; y < model.Height; y++)
            {
                CreateWorldText(parent, x + "," + y,
                    coordinateConverter.GridToWorldCenter(x, y, model.CellSize, model.Origin),
                    coordinateConverter.Forward);
                Debug.DrawLine(coordinateConverter.GridToWorld(x, y, model.CellSize, model.Origin),
                    coordinateConverter.GridToWorld(x, y + 1, model.CellSize, model.Origin), Color.white, duration);
                Debug.DrawLine(coordinateConverter.GridToWorld(x, y, model.CellSize, model.Origin),
                    coordinateConverter.GridToWorld(x + 1, y, model.CellSize, model.Origin), Color.white, duration);
            }
        }

        Debug.DrawLine(coordinateConverter.GridToWorld(0, model.Height, model.CellSize, model.Origin),
            coordinateConverter.GridToWorld(model.Width, model.Height, model.CellSize, model.Origin), Color.white,
            duration);
        Debug.DrawLine(coordinateConverter.GridToWorld(model.Width, 0, model.CellSize, model.Origin),
            coordinateConverter.GridToWorld(model.Width, model.Height, model.CellSize, model.Origin), Color.white,
            duration);
    }

    private static TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir,
        int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center,
        int sortingOrder = 0)
    {
        GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = position;
        gameObject.transform.forward = dir;

        TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = fontSize;
        textMeshPro.color = color == default ? Color.white : color;
        textMeshPro.alignment = textAnchor;
        textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMeshPro;
    }
}