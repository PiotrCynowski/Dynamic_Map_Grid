using UnityEngine;

namespace SmartTiles
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
        public Texture2D[] groundTextures;
        public Texture2D blockTexture;
        public TextureAssign textureAssign;
        [Header("Tile Elements")]
        public GameObject[] tileElements;
        public int elementsSpacing = 3;
        public int maxElementsDensity = 50;
        [Header("Tile Layers")]
        public LayerMask layerMaskForPlayer;
        public LayerMask layerMaskForTiles;
        [Header("Tiles")]
        public int tileSize;
        public int tilesNumberDistanceFromPlayer;
        public int tilesNumberMaxDistanceFromCenter;
        public Material tileMat;

        public TilesConfig(Texture2D[] groundTextures, Texture2D blockTexture, GameObject[] tileElements)
        {
            this.groundTextures = groundTextures;
            this.blockTexture = blockTexture;
            this.tileElements = tileElements;
        }
    }

    public enum TextureAssign { random, byID, reveredByID };
}