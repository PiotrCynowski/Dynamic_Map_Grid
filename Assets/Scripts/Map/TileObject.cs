using System;
using UnityEngine;

namespace SmartTiles
{
    public class TileObject : MonoBehaviour
    {
        private Transform tileElementsContainer;
        private TileElements tileElements;

        public Vector2Int localPos;
        private Action<Vector2Int> PlayerIsOnTile = delegate { };
        private Vector2Int worldPos, playerLocalPos;
        private int endMapValue;
        private int tileSize, axisTilesNumber, tilesFromCorner;

        private BoxCollider endMapCollider;

        private LayerMask layerMask;

        public void Init(Vector2Int wPos, TileSettings settings, int tFromPlayer, Mesh tileMesh, Action<Vector2Int> playerColCallback, Material mat, Vector2Int? _localPos, int _endMapValue, LayerMask layerMask)
        {
            tileElementsContainer = new GameObject("elementsContainer").transform;
            tileElementsContainer.transform.parent = this.transform;

            tileElements = new(settings.tileSize, settings.elementsSpacing, settings.maxElementDensity, tileElementsContainer);

            BoxCollider collider1 = gameObject.AddComponent<BoxCollider>();
            collider1.size = new Vector3(settings.tileSize * 0.5f, 0.05f, settings.tileSize);

            BoxCollider collider2 = gameObject.AddComponent<BoxCollider>();
            collider2.size = new Vector3(settings.tileSize, 0.05f, settings.tileSize * 0.5f);

            endMapCollider = gameObject.AddComponent<BoxCollider>();
            endMapCollider.size = new Vector3(settings.tileSize, 0.5f, settings.tileSize);

            PlayerIsOnTile = playerColCallback;
            localPos = _localPos.HasValue ? _localPos.Value : new Vector2Int(wPos.x + tFromPlayer, wPos.y + tFromPlayer);

            endMapValue = _endMapValue;
            this.layerMask = layerMask;

            worldPos = wPos;
            playerLocalPos = new Vector2Int(tFromPlayer, tFromPlayer);
            tileSize = settings.tileSize;
            axisTilesNumber = (tFromPlayer * 2) + 1;
            tilesFromCorner = tFromPlayer * 2;
            gameObject.AddComponent<MeshFilter>().mesh = tileMesh;
            gameObject.AddComponent<MeshRenderer>().material = mat;
            tileElementsContainer.localPosition = new Vector3(-tileSize * 0.5f, 0, -tileSize * 0.5f);
            tileElements.Init(worldPos, mat, _localPos.HasValue, IsEndMapTile());
        }

        #region moving tiles
        public void MoveTiles(Vector2Int movedBy, Vector2Int playerWorldPos, bool isInPlayerRange)
        {
            if (isInPlayerRange)
            {
                localPos -= movedBy;
                return;
            }

            if (movedBy.x != 0 && movedBy.y != 0 && (!IsCorner(movedBy)))
            {
                if ((movedBy.x > 0 && localPos.x == 0) || (movedBy.x < 0 && localPos.x == tilesFromCorner))
                {
                    localPos.x += movedBy.x * tilesFromCorner;
                    localPos.y -= movedBy.y;
                    transform.position += new Vector3(movedBy.x * axisTilesNumber * tileSize, 0, 0);
                }
                else if ((movedBy.y > 0 && localPos.y == 0) || (movedBy.y < 0 && localPos.y == tilesFromCorner))
                {
                    localPos.x -= movedBy.x;
                    localPos.y += movedBy.y * tilesFromCorner;
                    transform.position += new Vector3(0, 0, movedBy.y * axisTilesNumber * tileSize);
                }
            }
            else
            {
                localPos += movedBy * tilesFromCorner;
                transform.position += new Vector3(movedBy.x * axisTilesNumber * tileSize, 0, movedBy.y * axisTilesNumber * tileSize);
            }

            worldPos = playerWorldPos + (localPos - playerLocalPos);
            tileElements.Refresh(worldPos, IsEndMapTile());
        }

        private bool IsCorner(Vector2Int moved)
        {
            return localPos == new Vector2Int(moved.x > 0 ? 0 : tilesFromCorner, moved.y > 0 ? 0 : tilesFromCorner);
        }

        private bool IsEndMapTile()
        {
            if (worldPos.x < -endMapValue || worldPos.x > endMapValue || worldPos.y < -endMapValue || worldPos.y > endMapValue)
            {
                endMapCollider.enabled = true;
                return true;
            }
            else
            {
                endMapCollider.enabled = false;
                return false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & layerMask.value) == 0)
            {
                return;
            }

            PlayerIsOnTile?.Invoke(localPos);
        }
        #endregion
    }
}