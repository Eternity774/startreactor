using StartReactor.Features.Core;

namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// Handles lose state logic - restarting current level.
    /// </summary>
    public class LoseStateHandler : ILoseStateHandler
    {
        private readonly GameModel _gameModel;

        public LoseStateHandler(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void HandleLose()
        {
            // Restart current level
            _gameModel.ResetGame();
        }
    }
}

