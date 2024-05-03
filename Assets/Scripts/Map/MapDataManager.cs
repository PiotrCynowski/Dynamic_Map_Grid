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
                if(_player == null) {
                    _player = GameObject.FindGameObjectWithTag("Player").transform;
                }
                return _player;
            }
        }

        Dictionary<Vector2, Vector3[]> dataTileList = new();

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

        #region save load game file
        public void SaveTilesData() {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");

        }
        #endregion
    }
}

[System.Serializable]
public class TileInfo {
    public Vector2 tileData;
    public Vector3[] tileElementsData;
    public TileInfo(Vector2 vector2Value, Vector3[] vector3Array) {
        this.tileData = vector2Value;
        this.tileElementsData = vector3Array;
    }
}