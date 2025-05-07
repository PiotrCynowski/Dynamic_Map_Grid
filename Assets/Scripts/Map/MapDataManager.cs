using PoolSpawner;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameMap.Generator
{
    public class MapDataManager : MonoBehaviour
    {
        public static MapDataManager Instance;
        [Header("Tile Ground")]
        [SerializeField] private Texture2D[] groundTextures;
        [SerializeField] private Texture2D blockTexture;
        private int groundTexLen;

        [Header("Pool Elements")]
        [SerializeField] GameObject[] mapElements;
        public SpawnWithPool poolMapElements { get; private set; }

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
        private Vector2Int playerWorldPosition;

        private SaveLoad saveLoad;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            LoopXYRoad.OnPlayerWPosUpdate += UpdatePlayerWPos;

            saveLoad = new SaveLoad();


            AddObjectPool();
            groundTexLen = groundTextures.Length;
        }

        private void AddObjectPool()
        {
            poolMapElements = new();
            for (int i = 0; i < mapElements.Length; i++)
            {
                poolMapElements.AddPoolForGameObject(mapElements[i], i);
            }
        }

        private void UpdatePlayerWPos(Vector2Int wPos)
        {
            playerWorldPosition = wPos;
        }

        private void OnApplicationQuit()
        {
            saveLoad.SaveTilesData();
        }

        #region Get Map Elements
        public (int, Texture2D) GetRndGround()
        {
            int groundID = Random.Range(0, groundTexLen);
            return (groundID, groundTextures[groundID]);
        }

        public Texture2D GetGroundByID(int ID)
        {
            return groundTextures[ID];
        }

        public Texture2D GetGroundBlock()
        {
            return blockTexture;
        }
        #endregion
    }
}