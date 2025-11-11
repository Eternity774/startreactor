using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.Core
{
	public class GameModel
	{
		public event Action<List<int>> SequenceGenerated;
		public event Action<int> SequenceValidated;
		public event Action SequenceFailed;
		public event Action RoundCompleted;
		public event Action LevelCompleted;

		private LevelConfiguration _currentLevel;
		private List<LevelConfiguration> _levels;
		private bool _isInitialized;

		private int _currentInputIndex;

		public GameConfiguration GameConfiguration { get; private set; }
		public LevelConfiguration CurrentLevel => _currentLevel;
		public List<LevelConfiguration> AllLevels => _levels;
		public int CurrentLevelIndex { get; private set; }
		public int CurrentSequenceIndex { get; private set; }
		public int CurrentSequenceLength { get; private set; }
		public List<int> CurrentSequence { get; private set; }
		public bool IsWaitingForInput { get; private set; }

		private readonly ResourcesProvider _resourcesProvider;
		private readonly LevelsProvider _levelsProvider;
		private readonly Random _random;

		public GameModel(ResourcesProvider resourcesProvider, LevelsProvider levelsProvider)
		{
			_resourcesProvider = resourcesProvider;
			_levelsProvider = levelsProvider;
			_random = new Random();
			CurrentSequence = new List<int>();
		}

		public async UniTask InitializeAsync(CancellationToken cancellationToken)
		{
			if (!_isInitialized)
			{
				(GameConfiguration, _levels) = await UniTask.WhenAll(
					_resourcesProvider.LoadAsync<GameConfiguration>(AddressableKeys.GameConfiguration, cancellationToken),
					_levelsProvider.LoadAllLevelsAsync(cancellationToken));

				cancellationToken.ThrowIfCancellationRequested();

				if (_levels.Count == 0)
				{
					throw new Exception("No levels found. At least one level must be configured.");
				}

				_isInitialized = true;
			}

			if (_currentLevel == null || CurrentLevelIndex < 0 || CurrentLevelIndex >= _levels.Count)
			{
				CurrentLevelIndex = 0;
				_currentLevel = _levels[0];
			}

			ResetGame();
		}

		public void SetLevel(int levelIndex)
		{
			CurrentLevelIndex = levelIndex;
			_currentLevel = _levels[levelIndex];
			ResetGame();
		}

		public void ResetGame()
		{
			CurrentSequenceIndex = 0;
			CurrentSequenceLength = _currentLevel.Sequences[0];
			CurrentSequence.Clear();
			_currentInputIndex = 0;
			IsWaitingForInput = false;
		}

		public void GenerateNewSequence()
		{
			CurrentSequence.Clear();
			_currentInputIndex = 0;
			IsWaitingForInput = false;

			var totalButtons = _currentLevel.GridSize.x * _currentLevel.GridSize.y;

			for (var i = 0; i < CurrentSequenceLength; i++)
			{
				var buttonIndex = _random.Next(0, totalButtons);
				CurrentSequence.Add(buttonIndex);
			}

			SequenceGenerated?.Invoke(CurrentSequence);
		}

		public void StartInputPhase()
		{
			_currentInputIndex = 0;
			IsWaitingForInput = true;
		}

		public bool ValidateInput(int colorIndex)
		{
			if (!IsWaitingForInput)
			{
				return false;
			}

			if (_currentInputIndex >= CurrentSequence.Count)
			{
				return false;
			}

			var isCorrect = CurrentSequence[_currentInputIndex] == colorIndex;

			if (isCorrect)
			{
				_currentInputIndex++;
				SequenceValidated?.Invoke(_currentInputIndex);

				if (_currentInputIndex >= CurrentSequence.Count)
				{
					IsWaitingForInput = false;

					CurrentSequenceIndex++;
					if (CurrentSequenceIndex < _currentLevel.Sequences.Count)
					{
						CurrentSequenceLength = _currentLevel.Sequences[CurrentSequenceIndex];
						RoundCompleted?.Invoke();
					}
					else
					{
						RoundCompleted?.Invoke();
						LevelCompleted?.Invoke();
					}

					return true;
				}
			}
			else
			{
				IsWaitingForInput = false;
				SequenceFailed?.Invoke();
			}

			return isCorrect;
		}

		public void ResetCurrentSequence()
		{
			_currentInputIndex = 0;
			IsWaitingForInput = false;
		}
	}
}