using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator
{
    public class LoopXYRoad : MonoBehaviour
    {
        [Header("Tiles")]
        [SerializeField] private int tileSize;
        [SerializeField] private int tilesNumberDistanceFromPlayer;
        [SerializeField] private int tilesNumberMaxDistanceFromCenter;
        [SerializeField] private LayerMask layerMaskForPlayer;
        [SerializeField] private LayerMask layerMaskForTiles;
        [SerializeField] private Material tileMat;

        [Header("Tile elements")]
        [SerializeField] private int elementsSpacing = 3;
        [SerializeField] private int maxElementsDensity = 50;

        private List<TileObject> tileDatas;
        private Vector2Int playerTileGPos, playerTileWorldPos, movedBy; //lastGPos;
        private bool isInPlayerRange;

        public delegate void playerWPosUpdated(Vector2Int wPos);
        public static event playerWPosUpdated OnPlayerWPosUpdate;

        private IEnumerator Start()
        {
            playerTileGPos = new Vector2Int(tilesNumberDistanceFromPlayer, tilesNumberDistanceFromPlayer);

            bool isNewGame = MapDataManager.Instance.saveLoad.LoadSaveGame();
            PrepareTilesAroundPlayer(isNewGame);
            MapDataManager.Instance.saveLoad.PreparePlayer(isNewGame);

            yield return null;
        }

        #region structure tiles position around player
        public void PrepareTilesAroundPlayer(bool isNewGame)
        {
            playerTileWorldPos = isNewGame ? Vector2Int.zero : MapDataManager.Instance.saveLoad.LoadPlayerWPos();
            Mesh tileMesh = new TileGenerator().GetNewTile(tileSize);
            TileSettings tileSettings = new(tileSize, elementsSpacing, maxElementsDensity);
            MapDataManager.Instance.saveLoad.PrepareTileSettings(tileSettings);

            GenerateTiles(center: playerTileWorldPos, tileSettings: tileSettings, tileMesh: tileMesh, useSavedPositions: !isNewGame);
        }

        private void GenerateTiles(Vector2Int center, TileSettings tileSettings, Mesh tileMesh, bool useSavedPositions)
        {
            tileDatas = new();

            for (int row = center.x - tilesNumberDistanceFromPlayer, lRow = 0; row <= center.x + tilesNumberDistanceFromPlayer; row++, lRow++)
            {
                for (int col = center.y - tilesNumberDistanceFromPlayer, lCol = 0; col <= center.y + tilesNumberDistanceFromPlayer; col++, lCol++)
                {
                    GameObject tile = new("Tile");
                    tile.transform.parent = gameObject.transform;
                    tile.transform.position = new Vector3(row * tileSettings.tileSize, 0, col * tileSettings.tileSize);

                    if (!SetTileLayer(tile))
                        return;

                    Material thisMat = new(tileMat);
                    Vector2Int? localCoords = useSavedPositions ? new Vector2Int(lRow, lCol) : null;

                    tile.AddComponent<TileObject>().Init(new Vector2Int(row, col), tileSettings, tilesNumberDistanceFromPlayer, tileMesh, PlayerStandsOnTile, thisMat, localCoords, tilesNumberMaxDistanceFromCenter, layerMaskForPlayer);

                    tileDatas.Add(tile.GetComponent<TileObject>());
                }
            }
        }

        private bool SetTileLayer(GameObject tile)
        {
            int maskValue = layerMaskForTiles.value;
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

        ///Callback for when the player stands on a new tile
        private void PlayerStandsOnTile(Vector2Int gPos)
        {
            if (playerTileGPos == gPos)
            {
                return;
            }

            movedBy = new Vector2Int(gPos.x - playerTileGPos.x, gPos.y - playerTileGPos.y);
            playerTileWorldPos += movedBy;
            OnPlayerWPosUpdate(playerTileWorldPos);

            for (int i = 0; i < tileDatas.Count; i++)
            {
                isInPlayerRange = Mathf.Abs(tileDatas[i].localPos.x - gPos.x) <= tilesNumberDistanceFromPlayer && Mathf.Abs(tileDatas[i].localPos.y - gPos.y) <= tilesNumberDistanceFromPlayer;
                tileDatas[i].MoveTiles(movedBy, playerTileWorldPos, isInPlayerRange);
            }
        }
        #endregion
    }

    public class TileGenerator
    {
        public Mesh GetNewTile(float tileSize)
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