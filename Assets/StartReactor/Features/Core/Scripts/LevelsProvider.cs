using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StartReactor.Features.Core
{
    /// <summary>
    /// LevelsProvider loads all level configurations from Addressables.
    /// Uses labels to load all levels at once.
    /// </summary>
    public class LevelsProvider
    {
        public async UniTask<List<LevelConfiguration>> LoadAllLevelsAsync(CancellationToken cancellationToken)
        {
            AsyncOperationHandle<IList<LevelConfiguration>> handle = Addressables.LoadAssetsAsync<LevelConfiguration>(AddressableKeys.Levels);
            
            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // Return levels as loaded (no sorting needed since LevelNumber was removed)
                    return handle.Result.ToList();
                }
                else
                {
                    Debug.LogError($"Failed to load levels. Status: {handle.Status}");
                    Addressables.Release(handle);
                    return new List<LevelConfiguration>();
                }
            }
            catch (System.OperationCanceledException)
            {
                Addressables.Release(handle);
                throw;
            }
        }

        /// <summary>
        /// Loads a specific level by its addressable key.
        /// </summary>
        public async UniTask<LevelConfiguration> LoadLevelAsync(string levelKey, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<LevelConfiguration> handle = 
                Addressables.LoadAssetAsync<LevelConfiguration>(levelKey);
            
            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
                else
                {
                    Debug.LogError($"Failed to load level: {levelKey}. Status: {handle.Status}");
                    Addressables.Release(handle);
                    return null;
                }
            }
            catch (System.OperationCanceledException)
            {
                Addressables.Release(handle);
                throw;
            }
        }
    }
}

