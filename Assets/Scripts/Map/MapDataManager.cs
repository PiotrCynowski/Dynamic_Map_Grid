using UnityEngine;

namespace SmartTiles
{
    public class MapDataManager : MonoBehaviour
    {
        public static MapDataManager Instance;

        public SaveLoad saveLoad;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            LoopXYRoad.OnPlayerWPosUpdate += UpdatePlayerWPos;

            saveLoad = new SaveLoad();
        }

        private void UpdatePlayerWPos(Vector2Int wPos)
        {
            saveLoad.playerWorldPosition = wPos;
        }

        private void OnApplicationQuit()
        {
            saveLoad.SaveTilesData();
        }

     
    }
}