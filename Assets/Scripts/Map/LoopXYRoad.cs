using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator {
    public class LoopXYRoad : MonoBehaviour {
        [Header("Tiles")]
        [SerializeField] private TileObject Tiles;
        [SerializeField] private int tileSize;
        [SerializeField] private int tilesNumberDistanceFromPlayer;

        [Header("Tile elements")]
        [SerializeField] private int elementsSpacing = 3;
        [SerializeField] private int maxElementsDensity = 50;

        private List<TileObject> tileDatas;
        private Vector2Int playerTileGPos, playerTileWorldPos, movedBy; //lastGPos;
        private bool isInPlayerRange;

        public delegate void playerWPosUpdated(Vector2Int wPos);
        public static event playerWPosUpdated OnPlayerWPosUpdate;

        private IEnumerator Start() {
            playerTileGPos = new Vector2Int(tilesNumberDistanceFromPlayer, tilesNumberDistanceFromPlayer);
           
            ///check save game
            if (MapDataManager.Instance.LoadSaveGame())
                LoadTilesPositionsAroundPlayer();
            else {
                GenerateTilesPositionsAroundPlayer();
                playerTileWorldPos = Vector2Int.zero;
            }

            yield return null;
        }

        #region structure tiles position around player
        public void GenerateTilesPositionsAroundPlayer() {
            TileGenerator tileGenerator = new();
            Mesh tileMesh = tileGenerator.GetNewTile(tileSize);

            TileSettings tileSettings = new(tileSize, elementsSpacing, maxElementsDensity);

            Material mat = Tiles.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            Material thisMat;
            GameObject tile;
            tileDatas = new();

            for (int row = -tilesNumberDistanceFromPlayer; row <= tilesNumberDistanceFromPlayer; row++) {
                for (int col = -tilesNumberDistanceFromPlayer; col <= tilesNumberDistanceFromPlayer; col++) {
                    tile = Instantiate(Tiles.gameObject, new Vector3(row * tileSize, 0, col * tileSize), Quaternion.identity, gameObject.transform);
                    thisMat = tile.GetComponentInChildren<MeshRenderer>().material = new(mat);
                    tile.GetComponent<TileObject>().Init(new Vector2Int(row, col), tileSettings, tilesNumberDistanceFromPlayer, tileMesh, PlayerStandsOnTile, thisMat, null);
                    tileDatas.Add(tile.GetComponent<TileObject>());
                }
            }

            MapDataManager.Instance.PrepareTileSettings(tileSettings);
            MapDataManager.Instance.PreparePlayer(true);
        }

        public void LoadTilesPositionsAroundPlayer() {
            TileSettings tileSettings = MapDataManager.Instance.GetTileSettings();
            playerTileWorldPos = MapDataManager.Instance.LoadPlayerWPos();

            TileGenerator tileGenerator = new();
            Mesh tileMesh = tileGenerator.GetNewTile(tileSettings.tileSize);

            Material mat = Tiles.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            Material thisMat;
            GameObject tile;
            tileDatas = new();

            for (int row = playerTileWorldPos.x - tilesNumberDistanceFromPlayer, lRow = 0; row <= playerTileWorldPos.x + tilesNumberDistanceFromPlayer; row++, lRow++) {
                for (int col = playerTileWorldPos.y - tilesNumberDistanceFromPlayer, lCol = 0; col <= playerTileWorldPos.y + tilesNumberDistanceFromPlayer; col++, lCol++) {
                    
                    tile = Instantiate(Tiles.gameObject, new Vector3(row * tileSettings.tileSize, 0, col * tileSettings.tileSize), Quaternion.identity, gameObject.transform);
                    thisMat = tile.GetComponentInChildren<MeshRenderer>().material = new(mat);
                    tile.GetComponent<TileObject>().Init(new Vector2Int(row, col), tileSettings, tilesNumberDistanceFromPlayer, tileMesh, PlayerStandsOnTile, thisMat, new Vector2Int(lRow, lCol));
                    tileDatas.Add(tile.GetComponent<TileObject>());
                }
            }

            MapDataManager.Instance.PreparePlayer(false);
        }

        ///Callback for when the player stands on a new tile
        private void PlayerStandsOnTile(Vector2Int gPos) {
            if(playerTileGPos == gPos) {
                return;
            }
              
            movedBy = new Vector2Int(gPos.x - playerTileGPos.x, gPos.y - playerTileGPos.y);
            playerTileWorldPos += movedBy;
            OnPlayerWPosUpdate(playerTileWorldPos);

            for (int i = 0; i < tileDatas.Count; i++) {

                isInPlayerRange = Mathf.Abs(tileDatas[i].localPos.x - gPos.x) <= tilesNumberDistanceFromPlayer
                           && Mathf.Abs(tileDatas[i].localPos.y - gPos.y) <= tilesNumberDistanceFromPlayer;

                tileDatas[i].MoveTiles(movedBy, playerTileWorldPos, isInPlayerRange);
            }
        }
        #endregion
    }

    public class TileGenerator {
       public Mesh GetNewTile(float tileSize) {
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