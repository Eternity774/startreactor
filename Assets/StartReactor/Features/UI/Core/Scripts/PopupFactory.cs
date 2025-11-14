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
			var popupPrefab = await _resourcesProvider.LoadAsync<GameObject>(addressableKey, cancellationToken);

			var popupInstance = Object.Instantiate(popupPrefab, container);

			var popup = popupInstance.GetComponent<T>();

			return popup;
		}
	}
}