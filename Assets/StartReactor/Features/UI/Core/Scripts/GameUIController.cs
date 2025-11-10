using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// GameUIController manages the UI updates and interactions for the Start Reactor game.
    /// It handles displaying game state information and UI feedback.
    /// </summary>
    public class GameUIController : ControllerBase
    {
        private readonly GameModel _gameModel;
        private readonly GameUIView _uiView;
        private readonly IGameEventsRequestsModel _gameEventsModel;

        private int _currentRound = 0;

        public GameUIController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            GameUIView uiView,
            IGameEventsRequestsModel gameEventsModel)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _uiView = uiView;
            _gameEventsModel = gameEventsModel;
        }

        protected override void OnStart()
        {
            _gameModel.SequenceGenerated += OnSequenceGenerated;
            _gameModel.RoundCompleted += OnRoundCompleted;
            _gameModel.GameOver += OnGameOver;

            if (_uiView.RestartButton != null)
            {
                _uiView.RestartButton.onClick.AddListener(OnRestartClicked);
            }

            UpdateUI();
        }

        protected override void OnStop()
        {
            _gameModel.SequenceGenerated -= OnSequenceGenerated;
            _gameModel.RoundCompleted -= OnRoundCompleted;
            _gameModel.GameOver -= OnGameOver;

            if (_uiView.RestartButton != null)
            {
                _uiView.RestartButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        private void OnSequenceGenerated(System.Collections.Generic.List<int> sequence)
        {
            _currentRound++;
            UpdateUI();
        }

        private void OnRoundCompleted()
        {
            UpdateUI();
        }

        private void OnGameOver()
        {
            if (_uiView.GameOverPanel != null)
            {
                _uiView.GameOverPanel.SetActive(true);
            }
        }

        private void OnRestartClicked()
        {
            if (_uiView.GameOverPanel != null)
            {
                _uiView.GameOverPanel.SetActive(false);
            }
            
            _currentRound = 0;
            _gameEventsModel.RequestRestart();
        }

        private void UpdateUI()
        {
            if (_uiView.RoundText != null)
            {
                _uiView.RoundText.text = $"Round: {_currentRound}";
            }

            if (_uiView.SequenceLengthText != null)
            {
                _uiView.SequenceLengthText.text = $"Sequence Length: {_gameModel.CurrentSequenceLength}";
            }

            if (_uiView.ScoreText != null)
            {
                int score = (_currentRound - 1) * 100;
                _uiView.ScoreText.text = $"Score: {score}";
            }
        }
    }
}

