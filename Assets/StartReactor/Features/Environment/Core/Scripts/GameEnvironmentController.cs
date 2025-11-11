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
        private readonly IGridFactory _gridFactory;
        private readonly IGridLayoutConfigurator _gridLayoutConfigurator;
        private readonly GameModel _gameModel;

        public GameEnvironmentController(
            IControllerFactory controllerFactory,
            IPlayfieldView playfieldView,
            IGridFactory gridFactory,
            IGridLayoutConfigurator gridLayoutConfigurator,
            GameModel gameModel)
            : base(controllerFactory)
        {
            _playfieldView = playfieldView;
            _gridFactory = gridFactory;
            _gridLayoutConfigurator = gridLayoutConfigurator;
            _gameModel = gameModel;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            await CreateGridForCurrentLevel(cancellationToken);
            
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cancellationToken);
            
            await ExecuteAndWaitResultAsync<SequenceController>(cancellationToken);
            
            if (_gameModel.CurrentSequenceIndex >= _gameModel.CurrentLevel.Sequences.Count)
            {
                await ExecuteAndWaitResultAsync<WinController>(cancellationToken);
            }
        }

        private async UniTask CreateGridForCurrentLevel(CancellationToken cancellationToken)
        {
            _playfieldView.ClearGrid();

            _gridLayoutConfigurator.ConfigureGridLayout(
                _gameModel.CurrentLevel.GridSize,
                _playfieldView.GridContainer);

            var buttons = await _gridFactory.CreateGridAsync(
                _gameModel.CurrentLevel,
                _playfieldView.GridContainer,
                cancellationToken);

            _playfieldView.InitializeButtons(buttons);
        }
    }
}

