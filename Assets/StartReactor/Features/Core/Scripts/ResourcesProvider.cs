using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StartReactor.Features.Core
{
    /// <summary>
    /// This class simplifies the process of loading resources asynchronously using Addressables, enhancing flexibility
    /// and performance in resource management tasks within game development.
    /// </summary>
    public class ResourcesProvider
    {
        /// <summary>
        /// Loads an asset asynchronously using Addressables by its addressable key.
        /// </summary>
        /// <typeparam name="T">The type of asset to load</typeparam>
        /// <param name="addressableKey">The addressable key/address of the asset</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>The loaded asset of type T</returns>
        public async UniTask<T> LoadAsync<T>(string addressableKey, CancellationToken cancellationToken) where T : Object
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(addressableKey);
            
            try
            {
                // Convert Addressables handle to UniTask with cancellation support
                await handle.ToUniTask(cancellationToken: cancellationToken);
                
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }
                else
                {
                    Debug.LogError($"Failed to load addressable asset: {addressableKey}. Status: {handle.Status}");
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

        /// <summary>
        /// Releases an asset that was loaded via Addressables.
        /// Call this when you're done with the asset to free memory.
        /// </summary>
        /// <typeparam name="T">The type of asset</typeparam>
        /// <param name="asset">The asset to release</param>
        public void Release<T>(T asset) where T : Object
        {
            if (asset != null)
            {
                Addressables.Release(asset);
            }
        }
    }
}

