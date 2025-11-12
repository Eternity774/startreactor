using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.UI
{
    public interface IPopupView
    {
        UniTask Show(CancellationToken cancellationToken);
        UniTask Hide(CancellationToken cancellationToken);
    }
}

