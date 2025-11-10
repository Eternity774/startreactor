using System;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// Interface for lose popup view.
    /// </summary>
    public interface ILosePopupView : IPopupView
    {
        event Action OnRestartClicked;
    }
}

