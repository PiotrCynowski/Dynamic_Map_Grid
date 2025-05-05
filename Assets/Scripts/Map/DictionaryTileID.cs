using System.Collections.Generic;
using UnityEngine;

public static class DictionaryTileID
{
    public static Dictionary<Vector2Int, int> tileIDPos = new Dictionary<Vector2Int, int>();

    public static int GetValue(Vector2Int localPos) {
        if (tileIDPos.TryGetValue(localPos, out var value)) {
            return value;
        }
        return 0; 
    }

    public static void SetValue(Vector2Int key, int value) {
        tileIDPos[key] = value;
    }
}