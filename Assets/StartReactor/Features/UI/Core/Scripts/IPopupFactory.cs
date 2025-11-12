using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.UI
{
    public interface IPopupFactory
    {
        UniTask<T> CreatePopupAsync<T>(string addressableKey, Transform container, CancellationToken cancellationToken) where T : class;
        void ReleasePopup<T>(T popup) where T : class, IPopupView;
    }
}

