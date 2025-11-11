using StartReactor.Features.Core;

namespace StartReactor.Features.Gameplay
{
    public class WinStateHandler : IWinStateHandler
    {
        private readonly GameModel _gameModel;

        public WinStateHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void HandleWin()
        {
            if (_gameModel.CurrentLevelIndex + 1 < _gameModel.AllLevels.Count)
            {
                _gameModel.SetLevel(_gameModel.CurrentLevelIndex + 1);
            }
        }
    }
}

