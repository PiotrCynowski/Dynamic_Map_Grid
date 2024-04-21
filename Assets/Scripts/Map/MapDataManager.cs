using PoolSpawner;
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

        #region Get Map Elements
        public Texture2D GetRndGround() {
            return groundTextures[Random.Range(0, groundTexLen)];
        }
        #endregion
    }
}