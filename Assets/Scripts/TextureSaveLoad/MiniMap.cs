using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] int tileDistanceFromCenter;
    [SerializeField] Transform minimapPanel;
    [SerializeField] GameObject minimapTile;
    GridLayoutGroup minimapGrid;

    void Start() {
        minimapGrid = minimapPanel.GetComponent<GridLayoutGroup>();

        BuildMiniMap();
    }

    void BuildMiniMap() {
        int squareMiniMap = (tileDistanceFromCenter * 2) + 1;
        minimapGrid.constraintCount = squareMiniMap;
        for (int x = 0; x < squareMiniMap; x++) {
            for (int y = 0; y < squareMiniMap; y++) {
                Instantiate(minimapTile, minimapPanel);
            }
        }
    }
}
