using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.Core
{
    /// <summary>
    /// Factory interface for creating popup views from Addressables.
    /// </summary>
    public interface IPopupFactory
    {
        UniTask<T> CreatePopupAsync<T>(string addressableKey, Transform container, CancellationToken cancellationToken) where T : class;
        void ReleasePopup(IPopupView popup);
    }
}

