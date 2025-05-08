using SmartTiles;
using UnityEngine;

public class SmartTiles_Tester : MonoBehaviour
{
    public SmartTilesConfig tilesConfig;

    public void Start()
    {
        LoopXYRoad.Instance.GenerateTiles(tilesConfig.GetConfig());
    }
}
