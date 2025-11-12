using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.UI
{
    public interface IWinPopupView : IPopupView
    {
        UniTask WaitForNextButtonClicked(CancellationToken token);
    }
}

