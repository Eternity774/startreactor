using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Environment;
using StartReactor.Features.UI;

namespace StartReactor.Features.Core
{
    public class GameLoopController : ControllerWithResultBase
    {
        private readonly GameModel _gameModel;
        private readonly IGameEventsModel _gameEventsModel;

        public GameLoopController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            IGameEventsModel gameEventsModel)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _gameEventsModel = gameEventsModel;
        }

        protected override void OnStart()
        {
            _gameEventsModel.RestartRequested += OnRestartRequested;
        }

        protected override void OnStop()
        {
            _gameEventsModel.RestartRequested -= OnRestartRequested;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            await _gameModel.InitializeAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            Execute<GameUIController>();
            ExecuteAndWaitResultAsync<GameEnvironmentController>(CancellationToken).Forget();
        }

        private void OnRestartRequested()
        {
            Complete();
        }
    }
}

