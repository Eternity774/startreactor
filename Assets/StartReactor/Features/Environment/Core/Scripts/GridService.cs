using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;

namespace StartReactor.Features.Environment
{
    public class GridService : IGridService
    {
        private readonly IGridFactory _gridFactory;
        private readonly IGridLayoutConfigurator _gridLayoutConfigurator;
        private readonly GameModel _gameModel;

        public GridService(
            IGridFactory gridFactory,
            IGridLayoutConfigurator gridLayoutConfigurator,
            GameModel gameModel)
        {
            _gridFactory = gridFactory;
            _gridLayoutConfigurator = gridLayoutConfigurator;
            _gameModel = gameModel;
        }

        public async UniTask CreateGrid(LevelConfiguration level, IPlayfieldView playfieldView, CancellationToken cancellationToken)
        {
            playfieldView.ClearGrid();

            _gridLayoutConfigurator.ConfigureGridLayout(
                level.GridSize,
                playfieldView.GridContainer);

            var buttons = await _gridFactory.CreateGridAsync(
                level,
                playfieldView.GridContainer,
                cancellationToken);

            playfieldView.InitializeButtons(buttons);
            playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.DisabledColor);
        }
    }
}

