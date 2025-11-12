using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StartReactor.Features.Core
{
    public class LevelsProvider
    {
        public async UniTask<List<LevelConfiguration>> LoadAllLevelsAsync(CancellationToken cancellationToken)
        {
            AsyncOperationHandle<LevelsConfig> handle = 
                Addressables.LoadAssetAsync<LevelsConfig>(AddressableKeys.LevelsConfig);
            
            try
            {
                await handle.ToUniTask(cancellationToken: cancellationToken);
                
                if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                {
                    LevelsConfig levelsConfig = handle.Result;
                    
                    List<LevelConfiguration> levels = levelsConfig.Levels
                        .Where(level => level != null)
                        .ToList();
                    
                    return levels;
                }
                else
                {
                    Debug.LogError($"Failed to load LevelsConfig. Status: {handle.Status}");
                    Addressables.Release(handle);
                    return new List<LevelConfiguration>();
                }
            }
            catch (System.OperationCanceledException)
            {
                Addressables.Release(handle);
                throw;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error loading LevelsConfig: {ex.Message}");
                Addressables.Release(handle);
                return new List<LevelConfiguration>();
            }
        }
    }
}

