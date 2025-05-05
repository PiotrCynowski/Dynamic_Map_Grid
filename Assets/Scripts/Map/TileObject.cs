using System;
using UnityEngine;

namespace GameMap.Generator
{
    public class TileObject : MonoBehaviour
    {
        [SerializeField] private Transform tileElementsContainer;
        private TileElements tileElements;

        public Vector2Int localPos, worldPos;
        private Action<Vector2Int> PlayerIsOnTile = delegate { };
        private Vector2Int playerLocalPos;
        private int endMapValue;
        private int tileSize, axisTilesNumber, tilesFromCorner;

        BoxCollider endMapCollider;

        public LayerMask layerMask;
        Material mapTile;

        public void Init(Vector2Int wPos, TileSettings settings, int tFromPlayer, Mesh tileMesh, Action<Vector2Int> playerColCallback, Material mat, Vector2Int? _localPos)
        {
            tileElements = new(settings.tileSize, settings.elementsSpacing, settings.maxElementDensity, tileElementsContainer);

            GetComponents<BoxCollider>()[0].size = new Vector3(settings.tileSize * 0.5f, 0.05f, settings.tileSize);
            GetComponents<BoxCollider>()[1].size = new Vector3(settings.tileSize, 0.05f, settings.tileSize * 0.5f);

            endMapCollider = GetComponentsInChildren<BoxCollider>()[2];
            endMapCollider.size = new Vector3(settings.tileSize, 0.5f, settings.tileSize);

            PlayerIsOnTile = playerColCallback;
            localPos = _localPos.HasValue ? _localPos.Value : new Vector2Int(wPos.x + tFromPlayer, wPos.y + tFromPlayer);

            worldPos = wPos;
            playerLocalPos = new Vector2Int(tFromPlayer, tFromPlayer);
            tileSize = settings.tileSize;
            axisTilesNumber = (tFromPlayer * 2) + 1;
            tilesFromCorner = tFromPlayer * 2;
            GetComponentInChildren<MeshFilter>().mesh = tileMesh;
            mapTile = mat;
            tileElementsContainer.localPosition = new Vector3(-tileSize * 0.5f, 0, -tileSize * 0.5f);
            tileElements.Init(worldPos, mat, _localPos.HasValue, isEndMapTile());
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
            tileElements.Refresh(worldPos, isEndMapTile());
        }

        private bool IsCorner(Vector2Int moved)
        {
            return localPos == new Vector2Int(moved.x > 0 ? 0 : tilesFromCorner, moved.y > 0 ? 0 : tilesFromCorner);
        }

        bool isEndMapTile()
        {
            if (worldPos.x < -endMapValue || worldPos.x > endMapValue || worldPos.y < -endMapValue || worldPos.y > endMapValue)
            {
                endMapCollider.enabled = true;
                SetMaterialTile(false);
                return true;
            }
            else
            {
                endMapCollider.enabled = false;
                SetMaterialTile(true);
                return false;
            }
        }

        void SetMaterialTile(bool isTile)
        {
            mapTile.SetInt("_tileID", isTile ? DictionaryTileID.GetValue(worldPos) : 0);
            mapTile.SetInt("_isTile", isTile ? 1 : 0);
        }

        void OnTriggerEnter(Collider other)
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