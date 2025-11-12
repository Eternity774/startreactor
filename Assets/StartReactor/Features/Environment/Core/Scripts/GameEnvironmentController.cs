using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Gameplay;

namespace StartReactor.Features.Environment
{
    public class GameEnvironmentController : ControllerWithResultBase
    {
        private readonly IPlayfieldView _playfieldView;
        private readonly IGridService _gridService;
        private readonly GameModel _gameModel;

        public GameEnvironmentController(
            IControllerFactory controllerFactory,
            IPlayfieldView playfieldView,
            IGridService gridService,
            GameModel gameModel)
            : base(controllerFactory)
        {
            _playfieldView = playfieldView;
            _gridService = gridService;
            _gameModel = gameModel;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            await _gridService.CreateGrid(_gameModel.CurrentLevel, _playfieldView, cancellationToken);
            await ExecuteAndWaitResultAsync<SequenceController>(cancellationToken);
            
            Complete();
        }
    }
}

