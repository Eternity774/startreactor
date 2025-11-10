using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.Core
{
	/// <summary>
	/// GameModel manages game logic for the Start Reactor sequence memory game.
	/// It handles sequence generation, validation, and game state management.
	/// </summary>
	public class GameModel
	{
		public event Action<List<int>> SequenceGenerated;
		public event Action<int> SequenceValidated;
		public event Action GameOver;
		public event Action RoundCompleted;
		public event Action LevelCompleted;

		private LevelConfiguration _currentLevel;
		private List<LevelConfiguration> _levels;

		private int _currentInputIndex;

		public GameConfiguration GameConfiguration { get; private set; }
		public LevelConfiguration CurrentLevel => _currentLevel;
		public List<LevelConfiguration> AllLevels => _levels;
		public int CurrentLevelIndex { get; private set; }
		public int CurrentSequenceIndex { get; private set; }
		public int CurrentSequenceLength { get; private set; }
		public List<int> CurrentSequence { get; private set; }
		public bool IsWaitingForInput { get; private set; }
		public bool IsGameOver { get; private set; }

		private readonly ResourcesProvider _resourcesProvider;
		private readonly LevelsProvider _levelsProvider;
		private readonly System.Random _random;

		public GameModel(ResourcesProvider resourcesProvider, LevelsProvider levelsProvider)
		{
			_resourcesProvider = resourcesProvider;
			_levelsProvider = levelsProvider;
			_random = new System.Random();
		}

		public async UniTask InitializeAsync(CancellationToken cancellationToken)
		{
			(GameConfiguration, _levels) = await UniTask.WhenAll(
				_resourcesProvider.LoadAsync<GameConfiguration>(AddressableKeys.GameConfiguration, cancellationToken),
				_levelsProvider.LoadAllLevelsAsync(cancellationToken));

			cancellationToken.ThrowIfCancellationRequested();

			if (_levels == null || _levels.Count == 0)
			{
				throw new Exception("No levels found. At least one level must be configured.");
			}

			// Start with first level
			CurrentLevelIndex = 0;
			_currentLevel = _levels[0];

			ResetGame();
		}

		public void SetLevel(int levelIndex)
		{
			if (_levels != null && levelIndex >= 0 && levelIndex < _levels.Count)
			{
				CurrentLevelIndex = levelIndex;
				_currentLevel = _levels[levelIndex];
				ResetGame();
			}
		}

		public void ResetGame()
		{
			if (_currentLevel == null || _currentLevel.Sequences == null || _currentLevel.Sequences.Count == 0)
			{
				throw new Exception("Current level has no sequences configured.");
			}
			
			CurrentSequenceIndex = 0;
			CurrentSequenceLength = _currentLevel.Sequences[0];
			CurrentSequence = new List<int>();
			_currentInputIndex = 0;
			IsWaitingForInput = false;
			IsGameOver = false;
		}

		public void GenerateNewSequence()
		{
			CurrentSequence.Clear();
			_currentInputIndex = 0;
			IsWaitingForInput = false;

			// Generate random button indices based on grid size
			int totalButtons = _currentLevel.GridSize.x * _currentLevel.GridSize.y;

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
			if (!IsWaitingForInput || IsGameOver)
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
					// Sequence completed successfully
					IsWaitingForInput = false;
					
					// Move to next sequence if available
					CurrentSequenceIndex++;
					if (CurrentSequenceIndex < _currentLevel.Sequences.Count)
					{
						// Next sequence in the level
						CurrentSequenceLength = _currentLevel.Sequences[CurrentSequenceIndex];
						RoundCompleted?.Invoke();
					}
					else
					{
						// All sequences completed - level finished
						RoundCompleted?.Invoke();
						LevelCompleted?.Invoke();
					}
					return true;
				}
			}
			else
			{
				// Wrong input - game over
				IsWaitingForInput = false;
				IsGameOver = true;
				GameOver?.Invoke();
			}

			return isCorrect;
		}
	}
}