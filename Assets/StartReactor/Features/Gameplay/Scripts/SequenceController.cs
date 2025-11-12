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
		private readonly IGridService _gridService;

		public SequenceController(
			IControllerFactory controllerFactory,
			GameModel gameModel,
			IPlayfieldView playfieldView,
			IGridService gridService)
			: base(controllerFactory)
		{
			_gameModel = gameModel;
			_playfieldView = playfieldView;
			_gridService = gridService;
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
			var currentLevelIndex = _gameModel.CurrentLevelIndex;

			while (!CancellationToken.IsCancellationRequested)
			{
				while (_gameModel.CurrentSequenceIndex < _gameModel.CurrentLevel.Sequences.Count)
				{
					await PlaySequenceRound(CancellationToken);

					if (_gameModel.CurrentLevelIndex != currentLevelIndex)
					{
						currentLevelIndex = _gameModel.CurrentLevelIndex;
						await RecreateGridForNextLevel(CancellationToken);
					}
				}

				Complete();
				return;
			}
		}

		private async UniTask RecreateGridForNextLevel(CancellationToken cancellationToken)
		{
			await _gridService.CreateGrid(_gameModel.CurrentLevel, _playfieldView, cancellationToken);
			await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.GridCreationDelay), cancellationToken: cancellationToken);
		}

		private async UniTask PlaySequenceRound(CancellationToken cancellationToken)
		{
			_gameModel.GenerateNewSequence();
			var initialSequenceIndex = _gameModel.CurrentSequenceIndex;

			var sequenceCompleted = false;

			while (!sequenceCompleted && !cancellationToken.IsCancellationRequested)
			{
				_playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.DisabledColor);

				await PlaySequence(_gameModel.CurrentSequence, cancellationToken);

				await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.ButtonDisplayDelay), cancellationToken: cancellationToken);

				_gameModel.StartInputPhase();
				_playfieldView.ResetAllButtonsToDefaultColor();

				while (_gameModel.IsWaitingForInput && !cancellationToken.IsCancellationRequested)
				{
					await UniTask.Yield();
				}

				if (_gameModel.CurrentSequenceIndex > initialSequenceIndex)
				{
					var isLastSequence = _gameModel.CurrentSequenceIndex >= _gameModel.CurrentLevel.Sequences.Count;

					await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.SequenceWinDelay), cancellationToken: cancellationToken);
					
					_playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.DisabledColor);
					
					if (isLastSequence)
					{
						await ExecuteAndWaitResultAsync<WinController>(cancellationToken);
					}
					else
					{
						await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.SequenceCompletionDelay), cancellationToken: cancellationToken);
					}

					sequenceCompleted = true;
				}
				else
				{
					_playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.ErrorColor);
					await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.ErrorFlashDuration), cancellationToken: cancellationToken);
					_playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.DisabledColor);
					_gameModel.ResetCurrentSequence();
				}
			}
		}

		private void OnButtonClicked(int buttonIndex)
		{
			if (!_gameModel.IsWaitingForInput)
			{
				return;
			}

			var isValid = _gameModel.ValidateInput(buttonIndex);

			if (isValid)
			{
				var isLastButton = !_gameModel.IsWaitingForInput;

				if (isLastButton)
				{
					_playfieldView.SetAllButtonsColor(_gameModel.GameConfiguration.CorrectColor);
				}
				else
				{
					_playfieldView.ShowButtonFeedback(buttonIndex,
						_gameModel.GameConfiguration.CorrectColor,
						_gameModel.GameConfiguration.ButtonColorFeedbackDuration);
				}
			}
		}

		private async UniTask PlaySequence(List<int> sequence, CancellationToken cancellationToken)
		{
			var sequenceColor = _gameModel.GameConfiguration.SequenceColor;
			var disabledColor = _gameModel.GameConfiguration.DisabledColor;

			foreach (var buttonIndex in sequence)
			{
				cancellationToken.ThrowIfCancellationRequested();

				_playfieldView.SetButtonColor(buttonIndex, sequenceColor);

				await UniTask.Delay(TimeSpan.FromSeconds(_gameModel.GameConfiguration.SequenceButtonHighlightDuration), cancellationToken: cancellationToken);

				_playfieldView.SetButtonColor(buttonIndex, disabledColor);

				await UniTask.Delay(
					TimeSpan.FromSeconds(_gameModel.GameConfiguration.ButtonDisplayDelay + _gameModel.GameConfiguration.SequenceButtonPostFeedbackDelay),
					cancellationToken: cancellationToken);
			}
		}
	}
}