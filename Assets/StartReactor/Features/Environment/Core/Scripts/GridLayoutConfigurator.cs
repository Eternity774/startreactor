using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// Configures grid layout for the playfield.
    /// </summary>
    public class GridLayoutConfigurator : IGridLayoutConfigurator
    {
        public void ConfigureGridLayout(Vector2Int gridSize, Transform container)
        {
            GridLayoutGroup gridLayoutGroup = container.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogWarning($"GridLayoutGroup component not found on container: {container.name}");
                return;
            }

            RectTransform containerRect = container as RectTransform;
            if (containerRect != null)
            {
                float containerWidth = containerRect.rect.width;
                float containerHeight = containerRect.rect.height;

                float cellWidth = (containerWidth - (gridLayoutGroup.spacing.x * (gridSize.x - 1))) / gridSize.x;
                float cellHeight = (containerHeight - (gridLayoutGroup.spacing.y * (gridSize.y - 1))) / gridSize.y;

                gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            }

            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = gridSize.x;
        }
    }
}

