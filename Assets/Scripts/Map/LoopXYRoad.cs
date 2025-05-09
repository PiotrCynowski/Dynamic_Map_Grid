using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartTiles
{
    public class LoopXYRoad : MonoBehaviour
    {
        private int tilesNumberDistanceFromPlayer;
        private List<TileObject> tileDatas;
        private Vector2Int playerTileGPos, playerTileWorldPos;

        public delegate void playerWPosUpdated(Vector2Int wPos);
        public static event playerWPosUpdated OnPlayerWPosUpdate;

        public SpawnWithPool poolMapElements { get; private set; }

        private TilesConfig config;
        private int groundTexLen;

        public static LoopXYRoad instance { get; private set; }
        public static LoopXYRoad Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoopXYRoad>();
                    if (instance == null)
                    {
                        GameObject singletonObj = new GameObject("LoopXYRoad");
                        instance = singletonObj.AddComponent<LoopXYRoad>();
                        DontDestroyOnLoad(singletonObj);
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public void GenerateTiles(TilesConfig config)
        {
            AddObjectPools(config.tileElements);
            this.config = config;
            groundTexLen = config.groundTextures.Length;

            tilesNumberDistanceFromPlayer = config.tilesNumberDistanceFromPlayer;
            playerTileGPos = new Vector2Int(config.tilesNumberDistanceFromPlayer, config.tilesNumberDistanceFromPlayer);

            TileGenerator tileGenerator = new TileGenerator(config, PlayerStandsOnTile);
            (Vector2Int playerCenter, List<TileObject> tiles)  = tileGenerator.PrepareTiles();
            playerTileWorldPos = playerCenter;
            tileDatas = tiles;
        }

        private void PlayerStandsOnTile(Vector2Int gPos)
        {
            if (playerTileGPos == gPos)
            {
                return;
            }

            Vector2Int movedBy = new Vector2Int(gPos.x - playerTileGPos.x, gPos.y - playerTileGPos.y);
            playerTileWorldPos += movedBy;
            OnPlayerWPosUpdate(playerTileWorldPos);

            for (int i = 0; i < tileDatas.Count; i++)
            {
                bool isInPlayerRange = Mathf.Abs(tileDatas[i].localPos.x - gPos.x) <= tilesNumberDistanceFromPlayer && Mathf.Abs(tileDatas[i].localPos.y - gPos.y) <= tilesNumberDistanceFromPlayer;
                tileDatas[i].MoveTiles(movedBy, playerTileWorldPos, isInPlayerRange);
            }
        }

        private void AddObjectPools(GameObject[] tileElements)
        {
            poolMapElements = new();
            for (int i = 0; i < tileElements.Length; i++)
            {
                poolMapElements.AddPoolForGameObject(tileElements[i], i);
            }
        }

        #region Get Map Elements
        public (int, Texture2D) GetRndGround()
        {
            int groundID = UnityEngine.Random.Range(0, groundTexLen);
            return (groundID, config.groundTextures[groundID]);
        }

        public Texture2D GetGroundByID(int ID)
        {
            return config.groundTextures[ID];
        }

        public Texture2D GetGroundBlock()
        {
            return config.blockTexture;
        }
        #endregion
    }

    public class TileGenerator
    {
        private List<TileObject> tileDatas;
        private Vector2Int playerTileWorldPos;
        private readonly TilesConfig config;
        private Action<Vector2Int> onPlayerStandsOnTile;

        public TileGenerator(TilesConfig config, Action<Vector2Int> onPlayerStandsOnTile)
        {
            this.config = config;
            this.onPlayerStandsOnTile = onPlayerStandsOnTile;
        }

        public (Vector2Int playerCenter, List<TileObject> tiles) PrepareTiles()
        {
            bool isNewGame = MapDataManager.Instance.saveLoad.LoadSaveGame();           
            MapDataManager.Instance.saveLoad.PreparePlayer(isNewGame);
            return PrepareTilesAroundPlayer(isNewGame);
        }

        private (Vector2Int playerCenter, List<TileObject> tiles) PrepareTilesAroundPlayer(bool isNewGame)
        {
            playerTileWorldPos = isNewGame ? Vector2Int.zero : MapDataManager.Instance.saveLoad.LoadPlayerWPos();
            Mesh tileMesh = GetNewTile(config.tileSize);
            TileSettings tileSettings = new(config.tileSize, config.elementsSpacing, config.maxElementsDensity);
            MapDataManager.Instance.saveLoad.PrepareTileSettings(tileSettings);

            return (playerTileWorldPos, GenerateTiles(center: playerTileWorldPos, tileSettings: tileSettings, tileMesh: tileMesh, useSavedPositions: !isNewGame));
        }

        private List<TileObject> GenerateTiles(Vector2Int center, TileSettings tileSettings, Mesh tileMesh, bool useSavedPositions)
        {
            tileDatas = new();

            for (int row = center.x - config.tilesNumberDistanceFromPlayer, lRow = 0; row <= center.x + config.tilesNumberDistanceFromPlayer; row++, lRow++)
            {
                for (int col = center.y - config.tilesNumberDistanceFromPlayer, lCol = 0; col <= center.y + config.tilesNumberDistanceFromPlayer; col++, lCol++)
                {
                    GameObject tile = new("Tile");
                    tile.transform.parent = LoopXYRoad.Instance.gameObject.transform;
                    tile.transform.position = new Vector3(row * tileSettings.tileSize, 0, col * tileSettings.tileSize);

                    if (!SetTileLayer(tile))
                        return null;

                    Material thisMat = new(config.tileMat);
                    Vector2Int? localCoords = useSavedPositions ? new Vector2Int(lRow, lCol) : null;

                    tile.AddComponent<TileObject>().Init(new Vector2Int(row, col), tileSettings, config.tilesNumberDistanceFromPlayer, tileMesh, onPlayerStandsOnTile, thisMat, localCoords, config.tilesNumberMaxDistanceFromCenter, config.layerMaskForPlayer);

                    tileDatas.Add(tile.GetComponent<TileObject>());
                }
            }
            return tileDatas;
        }

        private bool SetTileLayer(GameObject tile)
        {
            int maskValue = config.layerMaskForTiles.value;
            if ((maskValue & (maskValue - 1)) == 0 && maskValue != 0)
            {
                tile.layer = (int)Mathf.Log(maskValue, 2);
                return true;
            }
            else
            {
                Debug.LogError("layerMaskForTiles must contain exactly one layer.");
                return false;
            }
        }

        private Mesh GetNewTile(float tileSize)
        {
            Mesh mesh = new();

            float halfWidth = tileSize * 0.5f;
            float halfHeight = tileSize * 0.5f;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(-halfWidth, 0f, -halfHeight);
            vertices[1] = new Vector3(halfWidth, 0f, -halfHeight);
            vertices[2] = new Vector3(-halfWidth, 0f, halfHeight);
            vertices[3] = new Vector3(halfWidth, 0f, halfHeight);

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}