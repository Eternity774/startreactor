using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.UI;

namespace StartReactor.Features.Gameplay
{
    public class WinController : ControllerWithResultBase
    {
        private readonly IPopupFactory _popupFactory;
        private readonly IWinStateHandler _winStateHandler;
        private readonly IGameEventsRequestsModel _gameEventsModel;
        private readonly GameUIView _uiView;
        private IWinPopupView _winPopup;

        public WinController(
            IControllerFactory controllerFactory,
            IPopupFactory popupFactory,
            IWinStateHandler winStateHandler,
            IGameEventsRequestsModel gameEventsModel,
            GameUIView uiView)
            : base(controllerFactory)
        {
            _popupFactory = popupFactory;
            _winStateHandler = winStateHandler;
            _gameEventsModel = gameEventsModel;
            _uiView = uiView;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            _winPopup = await _popupFactory.CreatePopupAsync<IWinPopupView>(
                AddressableKeys.WinPopup,
                _uiView.PopupContainer,
                cancellationToken);

            _winPopup.Show();
            await WaitForNextClick(cancellationToken);

            _winPopup.Hide();
            _popupFactory.ReleasePopup(_winPopup);

            _winStateHandler.HandleWin();
            _gameEventsModel.RequestRestart();

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

