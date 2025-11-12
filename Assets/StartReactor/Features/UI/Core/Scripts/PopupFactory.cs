using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.UI
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

            GameObject popupInstance = Object.Instantiate(popupPrefab, container);
            
            T popup = popupInstance.GetComponent<T>();

            return popup;
        }

        public void ReleasePopup<T>(T popup) where T : class, IPopupView
        {
            if (popup is MonoBehaviour monoBehaviour)
            {
                Object.Destroy(monoBehaviour.gameObject);
            }
        }
    }
}
