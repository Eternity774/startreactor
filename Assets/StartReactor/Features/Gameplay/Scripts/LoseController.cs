using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.UI;

namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// LoseController handles the lose state, showing a lose popup and waiting for Restart button click.
    /// </summary>
    public class LoseController : ControllerWithResultBase
    {
        private readonly IPopupFactory _popupFactory;
        private readonly ILoseStateHandler _loseStateHandler;
        private readonly GameUIView _uiView;
        private ILosePopupView _losePopup;

        public LoseController(
            IControllerFactory controllerFactory,
            IPopupFactory popupFactory,
            ILoseStateHandler loseStateHandler,
            GameUIView uiView)
            : base(controllerFactory)
        {
            _popupFactory = popupFactory;
            _loseStateHandler = loseStateHandler;
            _uiView = uiView;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            // Create lose popup from factory
            _losePopup = await _popupFactory.CreatePopupAsync<ILosePopupView>(
                AddressableKeys.LosePopup,
                _uiView.PopupContainer,
                cancellationToken);

            if (_losePopup == null)
            {
                Complete();
                return;
            }

            // Show popup
            _losePopup.Show();

            // Wait for Restart button click
            await WaitForRestartClick(cancellationToken);

            // Cleanup
            if (_losePopup != null)
            {
                _losePopup.Hide();
                _popupFactory.ReleasePopup(_losePopup);
            }

            // Handle lose state (restart current level)
            _loseStateHandler.HandleLose();

            Complete();
        }

        private async UniTask WaitForRestartClick(CancellationToken cancellationToken)
        {
            bool restartClicked = false;

            void OnRestart()
            {
                restartClicked = true;
            }

            _losePopup.OnRestartClicked += OnRestart;

            try
            {
                while (!restartClicked && !cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Yield();
                }
            }
            finally
            {
                _losePopup.OnRestartClicked -= OnRestart;
            }
        }
    }
}

