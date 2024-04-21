using System;
using UnityEngine;

namespace GameMap.Generator {
    public class TileObject : MonoBehaviour {
        Action<Vector2Int> PlayerIsOnTile = delegate { };
        public Vector2Int localPos, worldPos;
        public int id;
        Vector2Int playerLocalPos;
        float tileSize;
        int axisTilesNumber, tilesFromCorner;
        [SerializeField] TileElements thisTileElements;

        public void Init(Vector2Int wPos, int _id, float _tileSize, int tFromPlayer, Action<Vector2Int> playerColCallback, Material mat, Vector2Int? _localPos) {
            PlayerIsOnTile = playerColCallback;              
            localPos = _localPos.HasValue ? _localPos.Value : new Vector2Int(wPos.x + tFromPlayer, wPos.y + tFromPlayer);
            worldPos = wPos;
            id = _id;
            playerLocalPos = new Vector2Int(tFromPlayer, tFromPlayer);
            tileSize = _tileSize;
            axisTilesNumber = (tFromPlayer * 2) + 1;
            tilesFromCorner = tFromPlayer * 2;
            thisTileElements.Init(worldPos == Vector2Int.zero);
            mat.SetTexture("_MainTex", MapDataManager.Instance.GetRndGround());
        }

        #region moving tiles
        public void MoveTiles(Vector2Int movedBy, Vector2Int playerWorldPos, bool isInPlayerRange) {
            if (isInPlayerRange) {
                localPos -= movedBy;
                return;
            }

            if (movedBy.x != 0 && movedBy.y != 0 && (!IsCorner(movedBy))) {
                if ((movedBy.x > 0 && localPos.x == 0) || (movedBy.x < 0 && localPos.x == tilesFromCorner)) {
                    localPos.x += movedBy.x * tilesFromCorner;
                    localPos.y -= movedBy.y;
                    transform.position += new Vector3(movedBy.x * axisTilesNumber * tileSize, 0, 0);
                }
                else if ((movedBy.y > 0 && localPos.y == 0) || (movedBy.y < 0 && localPos.y == tilesFromCorner)) {
                    localPos.x -= movedBy.x;
                    localPos.y += movedBy.y * tilesFromCorner;
                    transform.position += new Vector3(0, 0, movedBy.y * axisTilesNumber * tileSize);
                }
            }
            else {
                localPos += movedBy * tilesFromCorner;
                transform.position += new Vector3(movedBy.x * axisTilesNumber * tileSize, 0, movedBy.y * axisTilesNumber * tileSize);
            }

            worldPos = playerWorldPos + (localPos - playerLocalPos);
        }

        bool IsCorner(Vector2Int moved) {
            return localPos == new Vector2Int(moved.x > 0 ? 0 : tilesFromCorner, moved.y > 0 ? 0 : tilesFromCorner);
        }

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != 8) {
                return;
            }

            PlayerIsOnTile?.Invoke(localPos);
        }
        #endregion
    }
}