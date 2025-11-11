using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;
using UnityEngine;

namespace StartReactor.Features.Gameplay
{
    public class SequenceController : ControllerWithResultBase
    {
        private readonly GameModel _gameModel;
        private readonly IPlayfieldView _playfieldView;

        public SequenceController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            IPlayfieldView playfieldView)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _playfieldView = playfieldView;
        }

        protected override void OnStart()
        {
            _playfieldView.OnButtonClicked += OnButtonClicked;
            StartGameLoop().Forget();
        }

        protected override void OnStop()
        {
            _playfieldView.OnButtonClicked -= OnButtonClicked;
        }

        private async UniTaskVoid StartGameLoop()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                _gameModel.ResetGame();
                
                while (_gameModel.CurrentSequenceIndex < _gameModel.CurrentLevel.Sequences.Count)
                {
                    await PlaySequenceRoundAsync(CancellationToken);
                }
                
                Complete();
                return;
            }
        }

        private async UniTask PlaySequenceRoundAsync(CancellationToken cancellationToken)
        {
            _gameModel.GenerateNewSequence();
            int initialSequenceIndex = _gameModel.CurrentSequenceIndex;
            
            bool sequenceCompleted = false;
            
            while (!sequenceCompleted && !cancellationToken.IsCancellationRequested)
            {
                await PlaySequence(_gameModel.CurrentSequence, cancellationToken);
                
                _gameModel.StartInputPhase();
                
                while (_gameModel.IsWaitingForInput && !cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Yield();
                }
                
                if (_gameModel.CurrentSequenceIndex > initialSequenceIndex)
                {
                    if (_gameModel.CurrentSequenceIndex < _gameModel.CurrentLevel.Sequences.Count)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: cancellationToken);
                    }
                    
                    sequenceCompleted = true;
                }
                else
                {
                    await FlashAllButtons(_gameModel.GameConfiguration.ErrorColor, cancellationToken);
                    _gameModel.ResetCurrentSequence();
                }
            }
        }

        private void OnButtonClicked(int buttonIndex)
        {
            if (!_gameModel.IsWaitingForInput)
                return;

            bool isValid = _gameModel.ValidateInput(buttonIndex);
            
            if (isValid)
            {
                bool sequenceCompleted = !_gameModel.IsWaitingForInput;
                
                if (sequenceCompleted)
                {
                    FlashAllButtons(_gameModel.GameConfiguration.CorrectColor, CancellationToken).Forget();
                }
                else
                {
                    _playfieldView.SetButtonColor(buttonIndex, _gameModel.GameConfiguration.CorrectColor);
                    _playfieldView.ShowButtonFeedback(buttonIndex, _gameModel.GameConfiguration.CorrectColor, 0.2f);
                }
            }
        }

        private async UniTask FlashAllButtons(Color color, CancellationToken cancellationToken)
        {
            await _playfieldView.FlashAllButtons(color, _gameModel.GameConfiguration.ErrorFlashDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
        }

        private async UniTask PlaySequence(List<int> sequence, CancellationToken cancellationToken)
        {
            Color sequenceColor = _gameModel.GameConfiguration.SequenceColor;

            foreach (int buttonIndex in sequence)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                _playfieldView.SetButtonColor(buttonIndex, sequenceColor);
                
                await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.SequenceButtonHighlightDuration), cancellationToken: cancellationToken);
                
                _playfieldView.ShowButtonFeedback(buttonIndex, sequenceColor, 0.1f);
                
                await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.ButtonDisplayDelay + 0.1f), cancellationToken: cancellationToken);
            }
        }
    }
}

