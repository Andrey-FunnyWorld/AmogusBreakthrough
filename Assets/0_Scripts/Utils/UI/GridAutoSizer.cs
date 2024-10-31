using UnityEngine;
using UnityEngine.UI;

public class GridAutoSizer : BaseAutoSizer {
    public GridLayoutGroup Grid;
    public int SidePadding = 20;
    protected override void Adjust(int width, int height) {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        int cols = Grid.constraintCount;
        Grid.cellSize = new Vector2((parentRect.rect.width - SidePadding * 2 - Grid.spacing.x * (cols - 1)) / cols, Grid.cellSize.y);
    }
}
