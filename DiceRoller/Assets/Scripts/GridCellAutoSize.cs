using UnityEngine;
using UnityEngine.UI;

public class GridCellAutoSize : MonoBehaviour
{
    public GridLayoutGroup grid;
    public RectTransform rectTransform;
    public int columnCount = 5;

    void Start()
    {
        UpdateCellSize();
    }

    void UpdateCellSize()
    {
        float parentWidth = rectTransform.rect.width;
        float spacing = grid.spacing.x;

        float cellSize = (parentWidth - (spacing * (columnCount - 1))) / columnCount;

        grid.cellSize = new Vector2(cellSize, cellSize);
    }
}
