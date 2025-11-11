using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StartReactor.Features.Core
{
    public class ResourcesProvider
    {
        public async UniTask<T> LoadAsync<T>(string addressableKey, CancellationToken cancellationToken) where T : Object
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(addressableKey);
            
            try
            {
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

        public void Release<T>(T asset) where T : Object
        {
            Addressables.Release(asset);
        }
    }
}

