using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator {
    public class LoopXYRoad : MonoBehaviour {
        [SerializeField] GameObject Tiles;
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
            Material mat = Tiles.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            Material thisMat;
            GameObject tile;
            tileDatas = new();
            int ID = 0;

            for (int row = -tilesNumberFromPlayer; row <= tilesNumberFromPlayer; row++) {
                for (int col = -tilesNumberFromPlayer; col <= tilesNumberFromPlayer; col++) {
                    tile = Instantiate(Tiles, new Vector3(row * tileSize, 0, col * tileSize), Quaternion.identity, gameObject.transform);
                    thisMat = tile.GetComponentInChildren<MeshRenderer>().material = new(mat);    
                    tile.GetComponent<TileObject>().Init(new Vector2Int(row , col), ID, tileSize, tilesNumberFromPlayer, PlayerStandsOnTile, thisMat, null);
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
}