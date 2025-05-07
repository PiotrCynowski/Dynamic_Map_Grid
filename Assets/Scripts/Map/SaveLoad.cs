using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad
{
    private Dictionary<Vector2, TileData> dataTileList = new();
    private string filePath;
    private TileSettings tileSettings;
    private Transform _player;
    public Transform Player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            return _player;
        }
    }
    private Vector3 playerPosition;
    public Vector2Int playerWorldPosition;

    public SaveLoad()
    {
        filePath = Path.Combine(Application.persistentDataPath, "data.json");
    }

    #region Save Load Game Local
    public void PreparePlayer(bool isNewGame)
    {
        Player.transform.position = isNewGame ? Vector3.zero : playerPosition;
    }

    public void AddTileData(Vector2 tilePos, Vector3[] tileElementsData, int groundID)
    {
        dataTileList.Add(tilePos, new TileData(tilePos, tileElementsData, groundID));
    }

    public bool IsTileExist(Vector2 tilePos)
    {
        return dataTileList.ContainsKey(tilePos);
    }

    public (Vector3[], int groundID) GetTileData(Vector2 tilePos)
    {
        return (dataTileList[tilePos].tileElementsData, dataTileList[tilePos].groundID);
    }

    public Vector2Int LoadPlayerWPos()
    {
        return playerWorldPosition;
    }

    public void PrepareTileSettings(TileSettings newTileSettings)
    {
        tileSettings = newTileSettings;
    }

    public TileSettings GetTileSettings() { return tileSettings; }
    #endregion

    #region Save Game File
    public void SaveTilesData()
    {
        List<TileData> tileData = new List<TileData>();
        foreach (Vector2 key in dataTileList.Keys)
        {
            tileData.Add(dataTileList[key]);
        }

        SerializableTileInfoArray saveGame = new(tileData.ToArray(), Player.position, playerWorldPosition, tileSettings);
        string json = JsonUtility.ToJson(saveGame);
        File.WriteAllText(filePath, FormatJsonString(json));
    }

    private string FormatJsonString(string json)
    {
        int indentLevel = 0;
        var formattedJson = new StringWriter();
        char[] chars = json.ToCharArray();

        for (int i = 0, spaces = 0; i < chars.Length; i++)
        {
            switch (chars[i])
            {
                case '[':
                case '{':
                    formattedJson.Write(chars[i]);
                    formattedJson.WriteLine();
                    indentLevel++;
                    spaces = indentLevel * 4;
                    formattedJson.Write(new string(' ', spaces));
                    break;
                case ']':
                case '}':
                    formattedJson.WriteLine();
                    indentLevel--;
                    spaces = indentLevel * 4;
                    formattedJson.Write(new string(' ', spaces));
                    formattedJson.Write(chars[i]);
                    break;
                case ',':
                    formattedJson.Write(chars[i]);
                    formattedJson.WriteLine();
                    spaces = indentLevel * 4;
                    formattedJson.Write(new string(' ', spaces));
                    break;
                default:
                    formattedJson.Write(chars[i]);
                    break;
            }
        }

        return formattedJson.ToString();
    }
    #endregion

    #region Load Game File
    public bool LoadSaveGame()
    {
        if (!File.Exists(filePath))
        {
            return true;
        }

        string json = File.ReadAllText(filePath);
        SerializableTileInfoArray deserializedData = JsonUtility.FromJson<SerializableTileInfoArray>(json);

        if (deserializedData != null && deserializedData.tileInfoArray != null)
        {
            foreach (TileData tileData in deserializedData.tileInfoArray)
            {
                dataTileList.Add(tileData.tileData, tileData);
            }
        }

        PrepareTileSettings(deserializedData.tileSettings);
        playerWorldPosition = deserializedData.playerWorldPosition;
        playerPosition = deserializedData.playerPosition;

        return false;
    }
    #endregion
}
