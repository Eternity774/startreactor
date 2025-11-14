using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.Environment
{
	public class GridFactory : IGridFactory
	{
		private readonly ResourcesProvider _resourcesProvider;

		public GridFactory(ResourcesProvider resourcesProvider)
		{
			_resourcesProvider = resourcesProvider;
		}

		public async UniTask<List<PlayfieldButton>> CreateGridAsync(
			LevelConfiguration level,
			Transform container,
			CancellationToken cancellationToken)
		{
			var buttonPrefab = await _resourcesProvider.LoadAsync<GameObject>(AddressableKeys.PlayfieldButton, cancellationToken);

			var buttons = new List<PlayfieldButton>();
			var gridSize = level.GridSize;
			var buttonIndex = 0;

			for (var y = 0; y < gridSize.y; y++)
			{
				for (var x = 0; x < gridSize.x; x++)
				{
					var buttonInstance = Object.Instantiate(buttonPrefab, container);
					buttonInstance.name = $"PlayfieldButton_{x}_{y}";

					var playfieldButton = buttonInstance.GetComponent<PlayfieldButton>();

					playfieldButton.Initialize(buttonIndex);
					buttons.Add(playfieldButton);
					buttonIndex++;
				}
			}

			return buttons;
		}
	}
}