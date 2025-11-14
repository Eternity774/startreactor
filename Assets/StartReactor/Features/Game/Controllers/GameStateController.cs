using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;
using StartReactor.Features.Gameplay;
using StartReactor.Features.Presentation;

namespace StartReactor.Features.Game
{
	public class GameStateController : ControllerWithResultBase
	{
		private readonly GameModel _gameModel;
		private readonly IPlayfieldView _playfieldView;
		private readonly IGridService _gridService;
		private readonly ResourcesProvider _resourcesProvider;
		private readonly ILevelsProvider _levelsProvider;
		private readonly ISequenceGenerator _sequenceGenerator;
		private readonly VisualFeedbackController _visualFeedback;
		private readonly SequencePlaybackController _sequencePlayback;
		private readonly InputPhaseController _inputPhase;

		private GameConfiguration _gameConfiguration;
		private int _currentLevelIndex;

		public GameStateController(
			IControllerFactory controllerFactory,
			GameModel gameModel,
			IPlayfieldView playfieldView,
			IGridService gridService,
			ResourcesProvider resourcesProvider,
			ILevelsProvider levelsProvider,
			ISequenceGenerator sequenceGenerator,
			VisualFeedbackController visualFeedback,
			SequencePlaybackController sequencePlayback,
			InputPhaseController inputPhase)
			: base(controllerFactory)
		{
			_gameModel = gameModel;
			_playfieldView = playfieldView;
			_gridService = gridService;
			_resourcesProvider = resourcesProvider;
			_levelsProvider = levelsProvider;
			_sequenceGenerator = sequenceGenerator;
			_visualFeedback = visualFeedback;
			_sequencePlayback = sequencePlayback;
			_inputPhase = inputPhase;
		}

		protected override void OnStart()
		{
			_gameModel.SequenceStartRequested += OnSequenceStartRequested;
			_currentLevelIndex = _levelsProvider.CurrentLevelIndex;
			_inputPhase.Start();
		}

		protected override void OnStop()
		{
			_gameModel.SequenceStartRequested -= OnSequenceStartRequested;
			_inputPhase.Stop();
		}

		protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
		{
			_gameConfiguration = await _resourcesProvider.LoadAsync<GameConfiguration>(
				AddressableKeys.GameConfiguration,
				cancellationToken);

			_visualFeedback.Initialize(_gameConfiguration);

			await CreateGrid(cancellationToken);

			_gameModel.ResetGame();
		}

		private void OnSequenceStartRequested()
		{
			PlayRoundAsync().Forget();
		}

	private async UniTask PlayRoundAsync()
	{
		if (_levelsProvider.CurrentLevelIndex != _currentLevelIndex)
		{
			_currentLevelIndex = _levelsProvider.CurrentLevelIndex;
			await CreateGrid(CancellationToken);
		}

		var isLastSequence = _gameModel.SequenceState.CurrentSequenceIndex >= _levelsProvider.CurrentLevel.Sequences.Count - 1;

		await PlayRound(CancellationToken);

		if (isLastSequence)
		{
			return;
		}

		_gameModel.RequestNextSequence();
	}

		private async UniTask CreateGrid(CancellationToken cancellationToken)
		{
			_playfieldView.ClearGrid();

			var playfieldButtons = await _gridService.CreateGrid(
				_levelsProvider.CurrentLevel,
				_playfieldView.GridContainer,
				cancellationToken);

			_playfieldView.InitializeButtons(playfieldButtons);
			_visualFeedback.SetAllButtonsDisabled();

			await UniTask.Delay(
				TimeSpan.FromSeconds(_gameConfiguration.DelayAfterGridCreated),
				cancellationToken: cancellationToken);
		}

	private async UniTask PlayRound(CancellationToken cancellationToken)
	{
		GenerateNewSequence();

		while (!cancellationToken.IsCancellationRequested)
		{
			await _sequencePlayback.PlaySequence(_gameModel.SequenceState.CurrentSequence, cancellationToken);

			await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.ButtonDisplayDelay), cancellationToken: cancellationToken);

			var inputResult = await _inputPhase.WaitForInput(cancellationToken);

			if (inputResult == InputPhaseController.InputPhaseResult.Success)
			{
				await HandleSuccessResult(cancellationToken);
				return;
			}

			await HandleFailureResult(cancellationToken);
		}
	}

	private async UniTask HandleSuccessResult(CancellationToken cancellationToken)
	{
		_gameModel.CompleteSequence();

		await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.SequenceWinDelay), cancellationToken: cancellationToken);

		_visualFeedback.SetAllButtonsDisabled();

		var isLastSequence = _gameModel.SequenceState.CurrentSequenceIndex >= _levelsProvider.CurrentLevel.Sequences.Count;

		if (isLastSequence)
		{
			await ExecuteAndWaitResultAsync<WinController>(cancellationToken);
		}
		else
		{
			await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.SequenceCompletionDelay), cancellationToken: cancellationToken);
		}
	}

		private async UniTask HandleFailureResult(CancellationToken cancellationToken)
		{
			await _visualFeedback.ShowErrorFlash(cancellationToken);

			_gameModel.SequenceState.ResetInput();
		}

		private void GenerateNewSequence()
		{
			var totalButtons = _levelsProvider.CurrentLevel.GridSize.x * _levelsProvider.CurrentLevel.GridSize.y;
			var currentIndex = _gameModel.SequenceState.CurrentSequenceIndex;

			if (currentIndex < 0 || currentIndex >= _levelsProvider.CurrentLevel.Sequences.Count)
			{
				return;
			}

			var sequenceLength = _levelsProvider.CurrentLevel.Sequences[currentIndex];

			var newSequence = _sequenceGenerator.GenerateSequence(totalButtons, sequenceLength);
			_gameModel.SequenceState.CurrentSequence = newSequence;
		}
	}
}