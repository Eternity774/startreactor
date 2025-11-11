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
            GameObject buttonPrefab = await _resourcesProvider.LoadAsync<GameObject>(
                AddressableKeys.PlayfieldButton, 
                cancellationToken);

            List<PlayfieldButton> buttons = new List<PlayfieldButton>();
            Vector2Int gridSize = level.GridSize;
            int buttonIndex = 0;

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    GameObject buttonInstance = Object.Instantiate(buttonPrefab, container);
                    buttonInstance.name = $"PlayfieldButton_{x}_{y}";
                    
                    PlayfieldButton playfieldButton = buttonInstance.GetComponent<PlayfieldButton>();

                    playfieldButton.Initialize(buttonIndex);
                    buttons.Add(playfieldButton);
                    buttonIndex++;
                }
            }

            return buttons;
        }
    }
}

