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
        [SerializeField] private GameObject[] tileElements;
        [SerializeField] private int elementsSpacing = 3;
        [SerializeField] private int maxElementsDensity = 50;
        [Header("Tile Layers")]
        [SerializeField] private LayerMask layerMaskForPlayer;
        [SerializeField] private LayerMask layerMaskForTiles;
        [Header("Tiles")]
        [SerializeField] private int tileSize;
        [SerializeField] private int tilesNumberDistanceFromPlayer;
        [SerializeField] private int tilesNumberMaxDistanceFromCenter;
        [SerializeField] private Material tileMat;

        public TilesConfig(Texture2D[] groundTextures, Texture2D blockTexture, GameObject[] tileElements)
        {
            this.groundTextures = groundTextures;
            this.blockTexture = blockTexture;
            this.tileElements = tileElements;
        }
    }
}