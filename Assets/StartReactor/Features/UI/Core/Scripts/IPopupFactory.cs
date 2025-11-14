using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.UI
{
    public interface IPopupFactory
    {
        UniTask<T> CreatePopupAsync<T>(string addressableKey, Transform container, CancellationToken cancellationToken) where T : class;
    }
}

