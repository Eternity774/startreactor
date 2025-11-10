using System;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// Interface for win popup view.
    /// </summary>
    public interface IWinPopupView : IPopupView
    {
        event Action OnNextClicked;
    }
}

