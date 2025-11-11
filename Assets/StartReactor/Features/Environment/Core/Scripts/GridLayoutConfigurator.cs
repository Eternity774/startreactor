using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    public class GridLayoutConfigurator : IGridLayoutConfigurator
    {
        public void ConfigureGridLayout(Vector2Int gridSize, Transform container)
        {
            GridLayoutGroup gridLayoutGroup = container.GetComponent<GridLayoutGroup>();

            RectTransform containerRect = container as RectTransform;
            float containerWidth = containerRect.rect.width;
            float containerHeight = containerRect.rect.height;

            float cellWidth = (containerWidth - (gridLayoutGroup.spacing.x * (gridSize.x - 1))) / gridSize.x;
            float cellHeight = (containerHeight - (gridLayoutGroup.spacing.y * (gridSize.y - 1))) / gridSize.y;

            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = gridSize.x;
        }
    }
}

