using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;

namespace StartReactor.Features.Gameplay
{
    public class WinController : ControllerWithResultBase
    {
        private readonly GameModel _gameModel;
        private readonly IGameEventsRequestsModel _gameEventsModel;

        public WinController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            IGameEventsRequestsModel gameEventsModel)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _gameEventsModel = gameEventsModel;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            if (_gameModel.CurrentLevelIndex + 1 < _gameModel.AllLevels.Count)
            {
                _gameModel.SetLevel(_gameModel.CurrentLevelIndex + 1);
            }

            await ExecuteAndWaitResultAsync<WinPopupController>(cancellationToken);

            _gameEventsModel.RequestRestart();

            Complete();
        }
    }
}

