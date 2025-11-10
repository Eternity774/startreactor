using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;
using UnityEngine;

namespace StartReactor.Features.Gameplay
{
    /// <summary>
    /// SequenceController manages the sequence playback and input validation for the Start Reactor game.
    /// It handles displaying the color sequence to the player and validating their input.
    /// </summary>
    public class SequenceController : ControllerWithResultBase
    {
        private readonly GameModel _gameModel;
        private readonly IPlayfieldView _playfieldView;
        private readonly IGameEventsRequestsModel _gameEventsModel;

        public SequenceController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            IPlayfieldView playfieldView,
            IGameEventsRequestsModel gameEventsModel)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _playfieldView = playfieldView;
            _gameEventsModel = gameEventsModel;
        }

        protected override void OnStart()
        {
            _gameModel.SequenceGenerated += OnSequenceGenerated;
            _gameModel.RoundCompleted += OnRoundCompleted;
            _gameModel.GameOver += OnGameOver;
            _playfieldView.OnButtonClicked += OnButtonClicked;

            StartGameLoop().Forget();
        }

        protected override void OnStop()
        {
            _gameModel.SequenceGenerated -= OnSequenceGenerated;
            _gameModel.RoundCompleted -= OnRoundCompleted;
            _gameModel.GameOver -= OnGameOver;
            _playfieldView.OnButtonClicked -= OnButtonClicked;
        }

        private async UniTaskVoid StartGameLoop()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                _gameModel.ResetGame();
                
                // Play all sequences for the current level
                while (_gameModel.CurrentSequenceIndex < _gameModel.CurrentLevel.Sequences.Count && !_gameModel.IsGameOver)
                {
                    await PlaySequenceRoundAsync(CancellationToken);
                    
                    if (_gameModel.IsGameOver)
                    {
                        break;
                    }
                }
                
                if (_gameModel.IsGameOver)
                {
                    // Game over - complete to trigger LoseController
                    Complete();
                    return;
                }
                
                // All sequences completed - level finished
                // Complete to trigger WinController
                Complete();
                return;
            }
        }

        private async UniTask PlaySequenceRoundAsync(CancellationToken cancellationToken)
        {
            // Generate and play sequence
            _gameModel.GenerateNewSequence();
            await PlaySequenceAsync(_gameModel.CurrentSequence, cancellationToken);
            
            // Wait a bit before allowing input
            await UniTask.Delay(500, cancellationToken: cancellationToken);
            
            // Start input phase
            _gameModel.StartInputPhase();
            _playfieldView.SetAllButtonsInteractable(true);
        }

        private async UniTask PlaySequenceAsync(List<int> sequence, CancellationToken cancellationToken)
        {
            _playfieldView.SetAllButtonsInteractable(false);

            foreach (int buttonIndex in sequence)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Highlight the button
                _playfieldView.SetButtonHighlight(buttonIndex, true, 1.5f);
                
                // Use general game configuration for display timing
                await UniTask.Delay((int)(_gameModel.GameConfiguration.ButtonDisplayDuration * 1000), cancellationToken: cancellationToken);
                
                // Unhighlight
                _playfieldView.SetButtonHighlight(buttonIndex, false, 1.5f);
                
                // Use general game configuration for display delay
                await UniTask.Delay((int)(_gameModel.GameConfiguration.ButtonDisplayDelay * 1000), cancellationToken: cancellationToken);
            }
        }

        private void OnSequenceGenerated(List<int> sequence)
        {
            // Sequence generation is handled in PlayRoundAsync
        }

        private void OnRoundCompleted()
        {
            HandleRoundCompletedAsync().Forget();
        }

        private async UniTaskVoid HandleRoundCompletedAsync()
        {
            _playfieldView.SetAllButtonsInteractable(false);
            
            await UniTask.Delay(1000, cancellationToken: CancellationToken);
        }

        private void OnGameOver()
        {
            _playfieldView.SetAllButtonsInteractable(false);
        }

        private void OnButtonClicked(int buttonIndex)
        {
            if (!_gameModel.IsWaitingForInput)
                return;

            bool isValid = _gameModel.ValidateInput(buttonIndex);
            
            // Show visual feedback
            Color feedbackColor = isValid 
                ? _gameModel.GameConfiguration.CorrectColor 
                : _gameModel.GameConfiguration.ErrorColor;
            
            _playfieldView.ShowButtonFeedback(buttonIndex, feedbackColor, 0.2f);
        }
    }
}

