using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameMap.Generator {
    public class TileElements {
        readonly int forestSize, elementSpacing, maxDensity;
        int density;
        Transform container;

        public TileElements(int forestSize, int elementSpacing, int maxDensity, Transform containerElements) {
            this.forestSize = forestSize;
            this.elementSpacing = elementSpacing;
            this.maxDensity = maxDensity;
            container = containerElements;
        }

        public void Init(Vector2Int worldPos) {
            if (worldPos != Vector2Int.zero) {
                density = Random.Range(0, maxDensity);
                GenerateElements(worldPos);
            }
        }

        public void Refresh(Vector2Int worldPos) {
            foreach (Transform obj in container) {
                if (obj.gameObject.activeSelf)
                    MapDataManager.Instance.poolMapElements.ThisObjReleased(obj.gameObject, 0);
            }
            density = Random.Range(0, maxDensity);

            if (MapDataManager.Instance.IsTileExist(worldPos))
                LoadElements(worldPos);
            else GenerateElements(worldPos);
        }

        void GenerateElements(Vector2Int worldPos) {
            List<Vector3> tileElements = new List<Vector3>();
            GameObject newElement;
            int containerX = (int)container.position.x;
            int containerZ = (int)container.position.z;
            int endX = containerX + forestSize;
            int endZ = containerZ + forestSize;

            for (int x = containerX; x < endX; x += elementSpacing) {
                for (int z = containerZ; z < endZ; z += elementSpacing) {
                    if (Random.Range(0, 100) < density) {
                        newElement = MapDataManager.Instance.poolMapElements.GetSpawnObject(0);
                        newElement.transform.SetParent(container);
                        //newElement.transform.position = new Vector3(x, 0f, z) + new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f)); ///position + offset
                        newElement.transform.position = new Vector3(x, 0f, z); ///position 
                        //newElement.transform.localScale = Vector3.one * Random.Range(0.9f, 1.4f); ///scale  
                        tileElements.Add(new Vector3(x, z, 1));
                    }
                }
            }

            MapDataManager.Instance.AddTileData(worldPos, tileElements.ToArray());
        }

        void LoadElements(Vector2Int worldPos) {
            Vector3[] tileData = MapDataManager.Instance.GetTileData(worldPos);
            GameObject newElement;

            for (int i = 0; i < tileData.Length; i++) {
                    newElement = MapDataManager.Instance.poolMapElements.GetSpawnObject(0);
                    newElement.transform.SetParent(container);
                    newElement.transform.position = new Vector3(tileData[i].x, 0f, tileData[i].y); ///position               
            }
        }
    }
}