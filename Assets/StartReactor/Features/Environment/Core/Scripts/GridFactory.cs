using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// GridFactory creates a dynamic grid of gameplay buttons based on level configuration.
    /// Buttons are loaded from Addressables and arranged in a grid layout.
    /// </summary>
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
            if (level == null)
            {
                throw new System.Exception("Level configuration is null.");
            }

            if (container == null)
            {
                throw new System.Exception("Container transform is null.");
            }

            // Load button prefab from Addressables
            GameObject buttonPrefab = await _resourcesProvider.LoadAsync<GameObject>(
                AddressableKeys.PlayfieldButton, 
                cancellationToken);
            
            if (buttonPrefab == null)
            {
                throw new System.Exception($"Failed to load button prefab with key: {AddressableKeys.PlayfieldButton}");
            }

            List<PlayfieldButton> buttons = new List<PlayfieldButton>();
            Vector2Int gridSize = level.GridSize;
            int buttonIndex = 0;

            // Create buttons in grid layout
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    GameObject buttonInstance = Object.Instantiate(buttonPrefab, container);
                    buttonInstance.name = $"PlayfieldButton_{x}_{y}";
                    
                    PlayfieldButton playfieldButton = buttonInstance.GetComponent<PlayfieldButton>();
                    if (playfieldButton == null)
                    {
                        Debug.LogError($"Button prefab is missing PlayfieldButton component!");
                        Object.Destroy(buttonInstance);
                        continue;
                    }

                    playfieldButton.Initialize(buttonIndex);
                    buttons.Add(playfieldButton);
                    buttonIndex++;
                }
            }

            return buttons;
        }
    }
}

