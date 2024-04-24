using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator {
    public class LoopXYRoad : MonoBehaviour {
        [SerializeField] TileObject Tiles;
        [SerializeField] float tileSize;
        [SerializeField] int tilesNumberFromPlayer;

        List<TileObject> tileDatas;
        Vector2Int playerTileGPos, playerTileWorldPos, movedBy, lastGPos;
        bool isInPlayerRange;

        IEnumerator Start() {
            playerTileGPos = new Vector2Int(tilesNumberFromPlayer, tilesNumberFromPlayer);
            playerTileWorldPos = Vector2Int.zero;
            GenerateTilesPositionsAroundPlayer();
            yield return null;
        }

        #region structure tiles position around player
        public void GenerateTilesPositionsAroundPlayer() {
            TileGenerator tileGenerator = new();
            Mesh tileMesh = tileGenerator.GetNewTile(tileSize);

            Material mat = Tiles.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            Material thisMat;
            GameObject tile;
            tileDatas = new();
            int ID = 0;

            for (int row = -tilesNumberFromPlayer; row <= tilesNumberFromPlayer; row++) {
                for (int col = -tilesNumberFromPlayer; col <= tilesNumberFromPlayer; col++) {
                    tile = Instantiate(Tiles.gameObject, new Vector3(row * tileSize, 0, col * tileSize), Quaternion.identity, gameObject.transform);
                    thisMat = tile.GetComponentInChildren<MeshRenderer>().material = new(mat);
                    thisMat.SetTexture("_MainTex", MapDataManager.Instance.GetRndGround());
                    tile.GetComponent<TileObject>().Init(new Vector2Int(row, col), ID, tileSize, tilesNumberFromPlayer, tileMesh, PlayerStandsOnTile, thisMat, null);
                    tileDatas.Add(tile.GetComponent<TileObject>());
                    ID++;
                }
            }
        }

        void PlayerStandsOnTile(Vector2Int gPos) {
            lastGPos = gPos;

            movedBy = new Vector2Int(gPos.x - playerTileGPos.x, gPos.y - playerTileGPos.y);
            playerTileWorldPos += movedBy;


            for (int i = 0; i < tileDatas.Count; i++) {

                isInPlayerRange = Mathf.Abs(tileDatas[i].localPos.x - gPos.x) <= tilesNumberFromPlayer
                           && Mathf.Abs(tileDatas[i].localPos.y - gPos.y) <= tilesNumberFromPlayer;

                tileDatas[i].MoveTiles(movedBy, playerTileWorldPos, isInPlayerRange);
            }
        }
        #endregion
    }

    public class TileGenerator {
       public Mesh GetNewTile(float tileSize) {
            Mesh mesh = new();

            float halfWidth = tileSize / 2f;
            float halfHeight = tileSize / 2f;

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