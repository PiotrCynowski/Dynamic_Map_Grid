using UnityEngine;

namespace GameMap.Generator {
    public class TileElements : MonoBehaviour {
        public void Init(bool isPlayerPos) {
            if (!isPlayerPos) 
                    GeneratePoints();
        }

        void GeneratePoints() { 

        }
    }
}