using UnityEngine;

[System.Serializable]
public class TileData
{
    public Vector2 tileData;
    public Vector3[] tileElementsData;
    public int groundID;

    public TileData(Vector2 vector2Value, Vector3[] vector3Array, int groundID)
    {
        this.tileData = vector2Value;
        this.tileElementsData = vector3Array;
        this.groundID = groundID;
    }
}

[System.Serializable]
public class SerializableTileInfoArray
{
    public TileData[] tileInfoArray;
    public Vector3 playerPosition;
    public Vector2Int playerWorldPosition;
    public TileSettings tileSettings;

    public SerializableTileInfoArray(TileData[] array, Vector3 playerPosition, Vector2Int playerWPos, TileSettings tileSettings)
    {
        this.tileInfoArray = array;
        this.playerPosition = playerPosition;
        this.playerWorldPosition = playerWPos;
        this.tileSettings = tileSettings;
    }
}

[System.Serializable]
public class TileSettings
{
    public int tileSize;
    public int elementsSpacing;
    public int maxElementDensity;

    public TileSettings(int tlSize, int elSpacing, int maxDensity)
    {
        tileSize = tlSize;
        elementsSpacing = elSpacing;
        maxElementDensity = maxDensity;
    }
}