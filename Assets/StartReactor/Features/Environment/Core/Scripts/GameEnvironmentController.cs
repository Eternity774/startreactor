using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Gameplay;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// GameEnvironmentController manages the game environment, creates the grid, and starts the sequence controller.
    /// Handles all logic related to grid setup and button management.
    /// </summary>
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
            // Create grid when level starts
            await CreateGridForCurrentLevel(cancellationToken);
            
            // Start sequence controller and wait for result
            await ExecuteAndWaitResultAsync<SequenceController>(cancellationToken);
            
            // Check game state and execute appropriate controller
            if (_gameModel.IsGameOver)
            {
                // Player lost - show lose popup
                await ExecuteAndWaitResultAsync<LoseController>(cancellationToken);
            }
            else if (_gameModel.CurrentSequenceIndex >= _gameModel.CurrentLevel.Sequences.Count)
            {
                // Player won - show win popup
                await ExecuteAndWaitResultAsync<WinController>(cancellationToken);
            }
        }

        private async UniTask CreateGridForCurrentLevel(CancellationToken cancellationToken)
        {
            if (_gameModel.CurrentLevel == null)
            {
                throw new System.Exception("Current level is null. Cannot create grid.");
            }

            // Clear existing grid
            _playfieldView.ClearGrid();

            // Configure grid layout
            _gridLayoutConfigurator.ConfigureGridLayout(
                _gameModel.CurrentLevel.GridSize,
                _playfieldView.GridContainer);

            // Create buttons
            var buttons = await _gridFactory.CreateGridAsync(
                _gameModel.CurrentLevel,
                _playfieldView.GridContainer,
                cancellationToken);

            // Initialize view with buttons (view only handles visual setup)
            _playfieldView.InitializeButtons(buttons);
        }
    }
}

