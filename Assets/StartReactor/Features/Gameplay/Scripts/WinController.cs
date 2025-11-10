using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.UI;

namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// WinController handles the win state, showing a win popup and waiting for Next button click.
    /// </summary>
    public class WinController : ControllerWithResultBase
    {
        private readonly IPopupFactory _popupFactory;
        private readonly IWinStateHandler _winStateHandler;
        private readonly GameUIView _uiView;
        private IWinPopupView _winPopup;

        public WinController(
            IControllerFactory controllerFactory,
            IPopupFactory popupFactory,
            IWinStateHandler winStateHandler,
            GameUIView uiView)
            : base(controllerFactory)
        {
            _popupFactory = popupFactory;
            _winStateHandler = winStateHandler;
            _uiView = uiView;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            // Create win popup from factory
            _winPopup = await _popupFactory.CreatePopupAsync<IWinPopupView>(
                AddressableKeys.WinPopup,
                _uiView.PopupContainer,
                cancellationToken);

            if (_winPopup == null)
            {
                Complete();
                return;
            }

            // Show popup
            _winPopup.Show();

            // Wait for Next button click
            await WaitForNextClick(cancellationToken);

            // Cleanup
            if (_winPopup != null)
            {
                _winPopup.Hide();
                _popupFactory.ReleasePopup(_winPopup);
            }

            // Handle win state (move to next level)
            _winStateHandler.HandleWin();

            Complete();
        }

        private async UniTask WaitForNextClick(CancellationToken cancellationToken)
        {
            bool nextClicked = false;

            void OnNext()
            {
                nextClicked = true;
            }

            _winPopup.OnNextClicked += OnNext;

            try
            {
                while (!nextClicked && !cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Yield();
                }
            }
            finally
            {
                _winPopup.OnNextClicked -= OnNext;
            }
        }
    }
}

