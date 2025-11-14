using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;
using Random = System.Random;

namespace StartReactor.Features.Gameplay
{
	public class SequenceController : ControllerWithResultBase
	{
		private enum SequencePhase
		{
			PlayingSequence,
			WaitingForInput,
			ProcessingResult
		}

		private readonly GameModel _gameModel;
		private readonly IPlayfieldView _playfieldView;
		private readonly IGridService _gridService;
		private readonly ResourcesProvider _resourcesProvider;
		private readonly ILevelsProvider _levelsProvider;

		private GameConfiguration _gameConfiguration;
		private SequencePhase _currentPhase = SequencePhase.PlayingSequence;
		private readonly Random _random = new();
		private bool _isPlayingSequenceRound;

		public SequenceController(
			IControllerFactory controllerFactory,
			GameModel gameModel,
			IPlayfieldView playfieldView,
			IGridService gridService,
			ResourcesProvider resourcesProvider,
			ILevelsProvider levelsProvider)
			: base(controllerFactory)
		{
			_gameModel = gameModel;
			_playfieldView = playfieldView;
			_gridService = gridService;
			_resourcesProvider = resourcesProvider;
			_levelsProvider = levelsProvider;
		}

		private int _currentLevelIndex;

		protected override void OnStart()
		{
			_playfieldView.OnButtonClicked += OnButtonClicked;
			_gameModel.SequenceShouldStart += OnSequenceShouldStart;
			_currentLevelIndex = _levelsProvider.CurrentLevelIndex;
		}

		private void OnSequenceShouldStart()
		{
			OnSequenceShouldStartAsync().Forget();
		}

		private async UniTask OnSequenceShouldStartAsync()
		{
			await UniTask.WaitUntil(() => !_isPlayingSequenceRound, cancellationToken: CancellationToken);

			_isPlayingSequenceRound = true;

			try
			{
				if (_levelsProvider.CurrentLevelIndex != _currentLevelIndex)
				{
					_currentLevelIndex = _levelsProvider.CurrentLevelIndex;
					await CreateGrid(CancellationToken);
				}

				await PlaySequenceRound(CancellationToken);
			}
			finally
			{
				_isPlayingSequenceRound = false;
			}
		}

		protected override void OnStop()
		{
			_playfieldView.OnButtonClicked -= OnButtonClicked;
			_gameModel.SequenceShouldStart -= OnSequenceShouldStart;
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			_gameConfiguration = await _resourcesProvider.LoadAsync<GameConfiguration>(AddressableKeys.GameConfiguration, cancellationToken);

			await CreateGrid(CancellationToken);
			
			_gameModel.ResetGame();
		}

		private async UniTask CreateGrid(CancellationToken cancellationToken)
		{
			_currentPhase = SequencePhase.PlayingSequence;
			_playfieldView.ClearGrid();

			var playfieldButtons = await _gridService.CreateGrid(_levelsProvider.CurrentLevel, _playfieldView.GridContainer, cancellationToken);

			_playfieldView.InitializeButtons(playfieldButtons);
			_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);

			await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.DelayAfterGridCreated), cancellationToken: cancellationToken);
		}


		private async UniTask PlaySequenceRound(CancellationToken cancellationToken)
		{
			_currentPhase = SequencePhase.PlayingSequence;
			var initialSequenceIndex = _gameModel.CurrentSequenceIndex;

			GenerateNewSequence();

			var expectedLength = _levelsProvider.CurrentLevel.Sequences[_gameModel.CurrentSequenceIndex];
			if (_gameModel.CurrentSequence.Count != expectedLength)
			{
				GenerateNewSequence();
			}

			for (var i = 0; i < int.MaxValue && !cancellationToken.IsCancellationRequested; i++)
			{
				_currentPhase = SequencePhase.PlayingSequence;
				_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);

				var sequenceToPlay = new List<int>(_gameModel.CurrentSequence);
				await PlaySequence(sequenceToPlay, cancellationToken);

				await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.ButtonDisplayDelay), cancellationToken: cancellationToken);

				_currentPhase = SequencePhase.WaitingForInput;
				_gameModel.StartInputPhase();
				_playfieldView.ResetAllButtonsToDefaultColor();

				while (_currentPhase == SequencePhase.WaitingForInput && !cancellationToken.IsCancellationRequested)
				{
					await UniTask.Yield();
				}

				_currentPhase = SequencePhase.ProcessingResult;

				if (_gameModel.CurrentSequenceIndex > initialSequenceIndex)
				{
					var isLastSequence = _gameModel.CurrentSequenceIndex >= _levelsProvider.CurrentLevel.Sequences.Count;

					await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.SequenceWinDelay), cancellationToken: cancellationToken);

					_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);

					if (isLastSequence)
					{
						await ExecuteAndWaitResultAsync<WinController>(cancellationToken);
					}
					else
					{
						await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.SequenceCompletionDelay), cancellationToken: cancellationToken);
					}

					return;
				}
				else
				{
					_playfieldView.SetAllButtonsColor(_gameConfiguration.ErrorColor);
					await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.ErrorFlashDuration), cancellationToken: cancellationToken);
					_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);
					_gameModel.ResetCurrentSequence();
					i--;
				}
			}
		}

		private void OnButtonClicked(int buttonIndex)
		{
			if (_currentPhase != SequencePhase.WaitingForInput)
			{
				return;
			}

			var validationResult = ValidateInput(buttonIndex);
			if (!validationResult.IsValid)
			{
				_currentPhase = SequencePhase.ProcessingResult;
				return;
			}

			if (validationResult.IsComplete)
			{
				_playfieldView.SetAllButtonsColor(_gameConfiguration.CorrectColor);
				_currentPhase = SequencePhase.ProcessingResult;
			}
			else
			{
				_playfieldView.ShowButtonFeedback(buttonIndex,
					_gameConfiguration.CorrectColor,
					_gameConfiguration.ButtonColorFeedbackDuration);
			}
		}

		private void GenerateNewSequence()
		{
			var totalButtons = _levelsProvider.CurrentLevel.GridSize.x * _levelsProvider.CurrentLevel.GridSize.y;
			var currentIndex = _gameModel.CurrentSequenceIndex;

			if (currentIndex < 0 || currentIndex >= _levelsProvider.CurrentLevel.Sequences.Count)
			{
				return;
			}

			var sequenceLength = _levelsProvider.CurrentLevel.Sequences[currentIndex];

			_gameModel.CurrentSequence.Clear();
			_gameModel.StartInputPhase();
			_gameModel.CurrentSequence.Capacity = sequenceLength;

			for (var i = 0; i < sequenceLength; i++)
			{
				var buttonIndex = _random.Next(0, totalButtons);
				_gameModel.CurrentSequence.Add(buttonIndex);
			}
		}

		private (bool IsValid, bool IsComplete) ValidateInput(int buttonIndex)
		{
			if (_gameModel.CurrentInputIndex >= _gameModel.CurrentSequence.Count)
			{
				return (false, false);
			}

			var isCorrect = _gameModel.CurrentSequence[_gameModel.CurrentInputIndex] == buttonIndex;
			if (!isCorrect)
			{
				return (false, false);
			}

			_gameModel.AdvanceInputIndex();

			if (_gameModel.CurrentInputIndex >= _gameModel.CurrentSequence.Count)
			{
				_gameModel.CompleteSequence();
				return (true, true);
			}

			return (true, false);
		}

		private async UniTask PlaySequence(List<int> sequence, CancellationToken cancellationToken)
		{
			foreach (var buttonIndex in sequence)
			{
				cancellationToken.ThrowIfCancellationRequested();

				_playfieldView.SetButtonColor(buttonIndex, _gameConfiguration.SequenceColor);

				await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.SequenceButtonHighlightDuration), cancellationToken: cancellationToken);

				_playfieldView.SetButtonColor(buttonIndex, _gameConfiguration.DisabledColor);

				await UniTask.Delay(
					TimeSpan.FromSeconds(_gameConfiguration.ButtonDisplayDelay + _gameConfiguration.SequenceButtonPostFeedbackDelay),
					cancellationToken: cancellationToken);
			}
		}
	}
}