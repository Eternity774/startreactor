using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.Core
{
    public class PopupFactory : IPopupFactory
    {
        private readonly ResourcesProvider _resourcesProvider;

        public PopupFactory(ResourcesProvider resourcesProvider)
        {
            _resourcesProvider = resourcesProvider;
        }

        public async UniTask<T> CreatePopupAsync<T>(string addressableKey, Transform container, CancellationToken cancellationToken) where T : class
        {
            GameObject popupPrefab = await _resourcesProvider.LoadAsync<GameObject>(
                addressableKey,
                cancellationToken);

            if (popupPrefab == null)
            {
                Debug.LogError($"Failed to load popup with key: {addressableKey}");
                return null;
            }

            GameObject popupInstance = Object.Instantiate(popupPrefab, container);
            
            T popup = popupInstance.GetComponent<T>();

            if (popup == null)
            {
                Debug.LogError($"Popup prefab is missing component of type {typeof(T).Name}!");
                Object.Destroy(popupInstance);
                return null;
            }

            return popup;
        }

        public void ReleasePopup(IPopupView popup)
        {
            if (popup != null && popup is MonoBehaviour monoBehaviour)
            {
                Object.Destroy(monoBehaviour.gameObject);
            }
        }
    }
}
