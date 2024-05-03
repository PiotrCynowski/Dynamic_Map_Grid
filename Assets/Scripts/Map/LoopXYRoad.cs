using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator {
    public class LoopXYRoad : MonoBehaviour {
        [Header("Tiles")]
        [SerializeField] TileObject Tiles;
        [SerializeField] float tileSize;
        [SerializeField] int tilesNumberDistanceFromPlayer;

        [Header("Tile elements")]
        [SerializeField] int elementsSpacing = 3;
        [SerializeField] int maxElementsDensity = 50;

        List<TileObject> tileDatas;
        Vector2Int playerTileGPos, playerTileWorldPos, movedBy; //lastGPos;
        bool isInPlayerRange;

        IEnumerator Start() {
            playerTileGPos = new Vector2Int(tilesNumberDistanceFromPlayer, tilesNumberDistanceFromPlayer);
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

            for (int row = -tilesNumberDistanceFromPlayer; row <= tilesNumberDistanceFromPlayer; row++) {
                for (int col = -tilesNumberDistanceFromPlayer; col <= tilesNumberDistanceFromPlayer; col++) {
                    tile = Instantiate(Tiles.gameObject, new Vector3(row * tileSize, 0, col * tileSize), Quaternion.identity, gameObject.transform);
                    thisMat = tile.GetComponentInChildren<MeshRenderer>().material = new(mat);
                    thisMat.SetTexture("_MainTex", MapDataManager.Instance.GetRndGround());
                    tile.GetComponent<TileObject>().Init(new Vector2Int(row, col), ID, tileSize, tilesNumberDistanceFromPlayer, tileMesh, PlayerStandsOnTile, thisMat, null, elementsSpacing, maxElementsDensity);
                    tileDatas.Add(tile.GetComponent<TileObject>());
                    ID++;
                }
            }
        }

        void PlayerStandsOnTile(Vector2Int gPos) {
            //lastGPos = gPos;

            movedBy = new Vector2Int(gPos.x - playerTileGPos.x, gPos.y - playerTileGPos.y);
            playerTileWorldPos += movedBy;


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