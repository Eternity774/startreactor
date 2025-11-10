using StartReactor.Features.Core;

namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// Handles win state logic - moving to next level.
    /// </summary>
    public class WinStateHandler : IWinStateHandler
    {
        private readonly GameModel _gameModel;

        public WinStateHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void HandleWin()
        {
            // Move to next level if available
            if (_gameModel.CurrentLevelIndex + 1 < _gameModel.AllLevels.Count)
            {
                _gameModel.SetLevel(_gameModel.CurrentLevelIndex + 1);
            }
        }
    }
}

