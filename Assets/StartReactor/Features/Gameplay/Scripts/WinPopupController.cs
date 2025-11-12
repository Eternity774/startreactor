using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.UI;

namespace StartReactor.Features.Gameplay
{
    public class WinPopupController : ControllerWithResultBase
    {
        private readonly IPopupFactory _popupFactory;
        private readonly GameUIView _uiView;
        private IWinPopupView _winPopup;

        public WinPopupController(
            IControllerFactory controllerFactory,
            IPopupFactory popupFactory,
            GameUIView uiView)
            : base(controllerFactory)
        {
            _popupFactory = popupFactory;
            _uiView = uiView;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            _winPopup = await _popupFactory.CreatePopupAsync<IWinPopupView>(
                AddressableKeys.WinPopup,
                _uiView.PopupContainer,
                cancellationToken);

            await _winPopup.Show(cancellationToken);
            await _winPopup.WaitForNextButtonClicked(cancellationToken);
            await _winPopup.Hide(cancellationToken);

            Complete();
        }
    }
}

