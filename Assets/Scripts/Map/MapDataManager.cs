using PoolSpawner;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator {
    public class MapDataManager : MonoBehaviour {
        public static MapDataManager Instance;
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

        public List<TileObject> currentTiles;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            }
            else {
                Instance = this;
            }

            AddObjectPool();
        }

        void AddObjectPool() {
            poolMapElements = new();
            for (int i = 0; i < mapElements.Length; i++) {
                poolMapElements.AddPoolForGameObject(mapElements[i], i);
            }
        }
    }
}