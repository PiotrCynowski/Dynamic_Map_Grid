using UnityEngine;

namespace GameMap.Generator {
    public class TileElements : MonoBehaviour {
        [SerializeField] int forestSize;
        [SerializeField] int elementSpacing;
        public int density;
        readonly int maxDensity = 50;

        public void Init(bool isPlayerPos) {
            if (!isPlayerPos) {
                density = Random.Range(0, maxDensity);
                GeneratePoints();
            }
        }

        void GeneratePoints() {
            GameObject newElement;
            float endX = transform.position.x + forestSize;
            float endZ = transform.position.z + forestSize;

            for (float x = transform.position.x; x < endX; x += elementSpacing) {
                for (float z = transform.position.z; z < endZ; z += elementSpacing) {

                    if (Random.Range(0, 100) < density) {
                        newElement = MapDataManager.Instance.poolMapElements.GetRandomSpawnObject();
                        newElement.transform.SetParent(this.transform);
                        newElement.transform.position = new Vector3(x, 0f, z) + new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f)); ///position + offset
                        newElement.transform.localScale = Vector3.one * Random.Range(0.9f, 1.4f); ///scale                                    
                    }
                }
            }
        }
    }
}