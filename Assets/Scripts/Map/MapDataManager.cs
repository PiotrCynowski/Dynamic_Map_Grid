using PoolSpawner;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameMap.Generator {
    public class MapDataManager : MonoBehaviour {
        public static MapDataManager Instance;
        [Header("Pool Elements")]
        [SerializeField] Texture2D[] groundTextures;
        int groundTexLen;

        [Header("Pool Elements")]
        [SerializeField] GameObject[] mapElements;
        public SpawnWithPool poolMapElements { get; private set; }

        [Header("ref")]
        private Transform _player;
        public Transform Player {
            get {
                if (_player == null) {
                    _player = GameObject.FindGameObjectWithTag("Player").transform;
                }
                return _player;
            }
        }
        Vector3 playerPosition;
        Vector2Int playerWorldPosition;
        public delegate void playerReady();
        public event playerReady OnPlayerReady;

        Dictionary<Vector2, TileData> dataTileList = new();
        TileSettings tileSettings;
        string filePath;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            }
            else {
                Instance = this;
            }

            LoopXYRoad.OnPlayerWPosUpdate += UpdatePlayerWPos;

            filePath = Path.Combine(Application.persistentDataPath, "data.json");
            AddObjectPool();
            groundTexLen = groundTextures.Length;
        }

        void AddObjectPool() {
            poolMapElements = new();
            for (int i = 0; i < mapElements.Length; i++) {
                poolMapElements.AddPoolForGameObject(mapElements[i], i);
            }
        }

        void UpdatePlayerWPos(Vector2Int wPos) {
            playerWorldPosition = wPos;
        }

        private void OnApplicationQuit() {
            SaveTilesData();
        }

        #region Get Map Elements
        public (int,Texture2D) GetRndGround() {
            int groundID = Random.Range(0, groundTexLen);
            return (groundID, groundTextures[groundID]);
        }

        public Texture2D GetGroundByID(int ID) {
            return groundTextures[ID];
        }
        #endregion

        #region save load game locally
        public void PreparePlayer(bool isNewGame) {
            Player.transform.position = isNewGame ? Vector3.zero : playerPosition;
            Player.GetComponent<Collider>().enabled = true;
        }

        public void AddTileData(Vector2 tilePos, Vector3[] tileElementsData, int groundID) {
            dataTileList.Add(tilePos, new TileData(tilePos, tileElementsData, groundID));
        }

        public bool IsTileExist(Vector2 tilePos) {
            return dataTileList.ContainsKey(tilePos);
        }

        public (Vector3[], int groundID) GetTileData(Vector2 tilePos) {
            return (dataTileList[tilePos].tileElementsData, dataTileList[tilePos].groundID);
        }

        public Vector2Int LoadPlayerWPos() {
            return playerWorldPosition;
        }

        public void PrepareTileSettings(TileSettings newTileSettings) {
            tileSettings = newTileSettings;
        }

        public TileSettings GetTileSettings() { return tileSettings; }
        #endregion

        #region save game file
        public void SaveTilesData() {
            List<TileData> tileData = new List<TileData>();
            foreach (Vector2 key in dataTileList.Keys) {
                tileData.Add(dataTileList[key]);
            }

            SerializableTileInfoArray saveGame = new(tileData.ToArray(), Player.position, playerWorldPosition, tileSettings);
            string json = JsonUtility.ToJson(saveGame);
            File.WriteAllText(filePath, FormatJsonString(json));
        }

        private string FormatJsonString(string json) {
            int indentLevel = 0;
            var formattedJson = new StringWriter();
            char[] chars = json.ToCharArray();

            for (int i = 0, spaces = 0; i < chars.Length; i++) {
                switch (chars[i]) {
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

        #region load game file
        public bool LoadSaveGame() {
            if (!File.Exists(filePath)) {
                return false;
            }
            
            string json = File.ReadAllText(filePath);
            SerializableTileInfoArray deserializedData = JsonUtility.FromJson<SerializableTileInfoArray>(json);

            if (deserializedData != null && deserializedData.tileInfoArray != null) {
                foreach (TileData tileData in deserializedData.tileInfoArray) {
                    dataTileList.Add(tileData.tileData, tileData);
                }
            }

            PrepareTileSettings(deserializedData.tileSettings);
            playerWorldPosition = deserializedData.playerWorldPosition;
            playerPosition = deserializedData.playerPosition;

            return true;
        }
        #endregion
    }
}

#region Save Game Data
[System.Serializable]
public class TileData {
    public Vector2 tileData;
    public Vector3[] tileElementsData;
    public int groundID;
    public TileData(Vector2 vector2Value, Vector3[] vector3Array, int groundID) {
        this.tileData = vector2Value;
        this.tileElementsData = vector3Array;
        this.groundID = groundID;
    }
}

[System.Serializable]
public class SerializableTileInfoArray {
    public TileData[] tileInfoArray;
    public Vector3 playerPosition;
    public Vector2Int playerWorldPosition;
    public TileSettings tileSettings;

    public SerializableTileInfoArray(TileData[] array, Vector3 playerPosition, Vector2Int playerWPos, TileSettings tileSettings) {
        this.tileInfoArray = array;
        this.playerPosition = playerPosition;
        this.playerWorldPosition = playerWPos;
        this.tileSettings = tileSettings;
    }
}

[System.Serializable]
public class TileSettings {
    public int tileSize;
    public int elementsSpacing;
    public int maxElementDensity;
    public TileSettings(int tlSize, int elSpacing, int maxDensity) {
        tileSize = tlSize;
        elementsSpacing = elSpacing;
        maxElementDensity = maxDensity;
    }
}
#endregion