using PoolSpawner;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        Dictionary<Vector2, Vector3[]> dataTileList = new();
        readonly string filePath = Path.Combine(Application.persistentDataPath, "data.json");

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            }
            else {
                Instance = this;
            }

            AddObjectPool();
            groundTexLen = groundTextures.Length;
        }

        void AddObjectPool() {
            poolMapElements = new();
            for (int i = 0; i < mapElements.Length; i++) {
                poolMapElements.AddPoolForGameObject(mapElements[i], i);
            }
        }

        private void OnApplicationQuit() {
            SaveTilesData();
        }

        #region Get Map Elements
        public Texture2D GetRndGround() {
            return groundTextures[Random.Range(0, groundTexLen)];
        }
        #endregion

        #region save load game locally
        public void AddTileData(Vector2 tilePos, Vector3[] tileElementsData) {
            dataTileList.Add(tilePos, tileElementsData);
        }

        public bool IsTileExist(Vector2 tilePos) {
            return dataTileList.ContainsKey(tilePos);
        }

        public Vector3[] GetTileData(Vector2 tilePos) {
            return dataTileList[tilePos];
        }
        #endregion

        #region save game file
        public void SaveTilesData() {
            List<TileData> tileData = new List<TileData>();
            foreach (Vector2 key in dataTileList.Keys) {
                tileData.Add(new TileData(key, dataTileList[key]));
            }

            SerializableTileInfoArray saveGame = new(tileData.ToArray(), Player.position);
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
        public void LoadSaveGame() {
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                SerializableTileInfoArray deserializedData = JsonUtility.FromJson<SerializableTileInfoArray>(json);

                if (deserializedData != null && deserializedData.tileInfoArray != null) {
                    foreach (TileData tileData in deserializedData.tileInfoArray) {
                        dataTileList.Add(tileData.tileData, tileData.tileElementsData);
                    }
                }
            }
        }
        #endregion
    }
}

        [System.Serializable]
public class TileData {
    public Vector2 tileData;
    public Vector3[] tileElementsData;
    public TileData(Vector2 vector2Value, Vector3[] vector3Array) {
        this.tileData = vector2Value;
        this.tileElementsData = vector3Array;
    }
}

[System.Serializable]
public class SerializableTileInfoArray {
    public TileData[] tileInfoArray;
    public Vector3 playerPosition;
    int tilesize;

    public SerializableTileInfoArray(TileData[] array, Vector3 playerPosition) {
        this.tileInfoArray = array;
        this.playerPosition = playerPosition;   
    }
}