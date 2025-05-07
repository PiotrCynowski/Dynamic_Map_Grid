using UnityEngine;

namespace TexturesNoiseMixer
{
    [CreateAssetMenu(fileName = "SmartTilesConfig", menuName = "ScriptableObjects/SmartTiles Config", order = 1)]
    public class SmartTilesConfig : ScriptableObject
    {
        [SerializeField] private TilesConfig Config;

        public TilesConfig GetConfig()
        {
            return Config;
        }
    }

    [System.Serializable]
    public class TilesConfig
    {
        [Header("Tile Ground")]
        [SerializeField] private Texture2D[] groundTextures;
        [SerializeField] private Texture2D blockTexture;
        [Header("Tile Elements")]
        [SerializeField] GameObject[] tileElements;

        public TilesConfig(Texture2D[] groundTextures, Texture2D blockTexture, GameObject[] tileElements)
        {
            this.groundTextures = groundTextures;
            this.blockTexture = blockTexture;
            this.tileElements = tileElements;
        }
    }
}