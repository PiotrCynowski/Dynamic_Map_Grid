using System.Collections.Generic;
using UnityEngine;

namespace GameMap.Generator
{
    public class TileElements
    {
        readonly private int forestSize, elementSpacing, maxDensity;
        private int density;
        private Transform container;
        private Dictionary<int, List<GameObject>> elements = new();
        private Material groundMat;

        public TileElements(int forestSize, int elementSpacing, int maxDensity, Transform containerElements)
        {
            this.forestSize = forestSize;
            this.elementSpacing = elementSpacing;
            this.maxDensity = maxDensity;
            container = containerElements;
        }

        public void Init(Vector2Int worldPos, Material groundMaterial, bool isLoading, bool isEndMap)
        {
            groundMat = groundMaterial;

            if (isLoading)
            {
                Refresh(worldPos, isEndMap);
                return;
            }

            if (isEndMap)
            {
                groundMat.SetTexture("_MainTex", MapDataManager.Instance.GetGroundBlock());
                return;
            }

            if (worldPos != Vector2Int.zero)
            {
                density = Random.Range(0, maxDensity);
                GenerateElements(worldPos);
            }
        }

        public void Refresh(Vector2Int worldPos, bool isEndMap)
        {
            foreach (int key in elements.Keys)
            {
                foreach (GameObject obj in elements[key])
                {
                    if (obj.activeSelf)
                        MapDataManager.Instance.poolMapElements.ThisObjReleased(obj, key);
                }
            }
            elements.Clear();

            if (isEndMap)
            {
                groundMat.SetTexture("_MainTex", MapDataManager.Instance.GetGroundBlock());
                return;
            }

            density = Random.Range(0, maxDensity);

            if (MapDataManager.Instance.IsTileExist(worldPos))
                LoadElements(worldPos);
            else
                GenerateElements(worldPos);
        }

        private void GenerateElements(Vector2Int worldPos)
        {
            List<Vector3> tileElements = new List<Vector3>();
            int containerX = (int)container.position.x;
            int containerZ = (int)container.position.z;
            int endX = containerX + forestSize;
            int endZ = containerZ + forestSize;

            for (int x = containerX; x < endX; x += elementSpacing)
            {
                for (int z = containerZ; z < endZ; z += elementSpacing)
                {
                    if (Random.Range(0, 100) < density)
                    {
                        (int ID, GameObject newElement) = MapDataManager.Instance.poolMapElements.GetRandomSpawnObject();
                        newElement.transform.SetParent(container);
                        newElement.transform.position = new Vector3(x, 0f, z); ///position 
                        tileElements.Add(new Vector3(x, z, ID));
                        AddElement(ID, newElement);
                    }
                }
            }

            (int gID, Texture2D ground) = MapDataManager.Instance.GetRndGround();
            groundMat.SetTexture("_MainTex", ground);
            MapDataManager.Instance.AddTileData(worldPos, tileElements.ToArray(), gID);
        }

        private void LoadElements(Vector2Int worldPos)
        {
            (Vector3[] tileData, int groundID) = MapDataManager.Instance.GetTileData(worldPos);
            GameObject newElement;
            int objID;

            for (int i = 0; i < tileData.Length; i++)
            {
                objID = (int)tileData[i].z;
                newElement = MapDataManager.Instance.poolMapElements.GetSpawnObject(objID);
                newElement.transform.SetParent(container);
                newElement.transform.position = new Vector3(tileData[i].x, 0f, tileData[i].y); ///position 
                AddElement(objID, newElement);
            }

            groundMat.SetTexture("_MainTex", MapDataManager.Instance.GetGroundByID(groundID));
        }

        private void AddElement(int ID, GameObject obj)
        {
            if (!elements.ContainsKey(ID))
                elements[ID] = new List<GameObject>();
            elements[ID].Add(obj);
        }
    }
}