using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
	public class GridService : IGridService
	{
		private readonly IGridFactory _gridFactory;

		public GridService(IGridFactory gridFactory)
		{
			_gridFactory = gridFactory;
		}

		public async UniTask<List<PlayfieldButton>> CreateGrid(LevelConfiguration level, Transform container, CancellationToken cancellationToken)
		{
			ConfigureGridLayout(level.GridSize, container);

			return await _gridFactory.CreateGridAsync(level, container, cancellationToken);
		}

		private void ConfigureGridLayout(Vector2Int gridSize, Transform container)
		{
			var gridLayoutGroup = container.GetComponent<GridLayoutGroup>();
			var containerRect = container.GetComponent<RectTransform>();

			var containerWidth = containerRect.rect.width;
			var containerHeight = containerRect.rect.height;

			var cellWidth = (containerWidth - gridLayoutGroup.spacing.x * (gridSize.x - 1)) / gridSize.x;
			var cellHeight = (containerHeight - gridLayoutGroup.spacing.y * (gridSize.y - 1)) / gridSize.y;

			gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

			gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			gridLayoutGroup.constraintCount = gridSize.x;
		}
	}
}