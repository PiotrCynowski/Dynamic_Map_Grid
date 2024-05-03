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

        public void Init(bool isPlayerPos) {
            if (!isPlayerPos) {
                density = Random.Range(0, maxDensity);
                GeneratePoints();
            }
        }

        void GeneratePoints() {
            float[,] densityMap = new float[forestSize, forestSize];

            GameObject newElement;
            float endX = container.position.x + forestSize;
            float endZ = container.position.z + forestSize;

            for (float x = container.position.x; x < endX; x += elementSpacing) {
                for (float z = container.position.z; z < endZ; z += elementSpacing) {

                    if (Random.Range(0, 100) < density) {
                        newElement = MapDataManager.Instance.poolMapElements.GetRandomSpawnObject();
                        newElement.transform.SetParent(container);
                        newElement.transform.position = new Vector3(x, 0f, z) + new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f)); ///position + offset
                        newElement.transform.localScale = Vector3.one * Random.Range(0.9f, 1.4f); ///scale                                    
                    }
                }
            }
        }
    }
}