using System;
using UnityEngine;

namespace GameMap.Generator {
    public class TileObject : MonoBehaviour {
        [SerializeField] Transform tileElementsParent;
        TileElements tileElements;

        Action<Vector2Int> PlayerIsOnTile = delegate { };
        public Vector2Int localPos, worldPos;
        Vector2Int playerLocalPos;
        int tileSize, axisTilesNumber, tilesFromCorner;
        bool isStartingTile = false;

        public void Init(Vector2Int wPos, TileSettings settings, int tFromPlayer, Mesh tileMesh, Action<Vector2Int> playerColCallback, Material mat, Vector2Int? _localPos) {     
            tileElements = new(settings.tileSize, settings.elementsSpacing, settings.maxElementDensity, tileElementsParent);

            ///rescale colliders
            GetComponents<BoxCollider>()[0].size = new Vector3(settings.tileSize * 0.5f, 0.05f, settings.tileSize);
            GetComponents<BoxCollider>()[1].size = new Vector3(settings.tileSize, 0.05f, settings.tileSize * 0.5f);

            PlayerIsOnTile = playerColCallback;
            localPos = _localPos.HasValue ? _localPos.Value : new Vector2Int(wPos.x + tFromPlayer, wPos.y + tFromPlayer);

            if (localPos == new Vector2Int(tFromPlayer, tFromPlayer)) {
                GetComponents<BoxCollider>()[0].enabled = false;
                GetComponents<BoxCollider>()[1].enabled = false;
                isStartingTile = true;
            }

            worldPos = wPos;
            playerLocalPos = new Vector2Int(tFromPlayer, tFromPlayer);
            tileSize = settings.tileSize;
            axisTilesNumber = (tFromPlayer * 2) + 1;
            tilesFromCorner = tFromPlayer * 2;
            tileElements.Init(worldPos, mat, _localPos.HasValue);
            GetComponentInChildren<MeshFilter>().mesh = tileMesh;
            tileElementsParent.localPosition = new Vector3(-tileSize * 0.5f, 0, -tileSize * 0.5f);
        }

        #region moving tiles
        public void MoveTiles(Vector2Int movedBy, Vector2Int playerWorldPos, bool isInPlayerRange) {
            if(isStartingTile){
                GetComponents<BoxCollider>()[0].enabled = true;
                GetComponents<BoxCollider>()[1].enabled = true;
                isStartingTile = false;
            }
                    
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
            tileElements.Refresh(worldPos);
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