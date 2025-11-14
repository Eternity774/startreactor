using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.Core
{
	public class GameModel
	{
		public event Action LevelCompleted;
		public event Action SequenceShouldStart;
		public event Action SequenceIndexChanged;

		private int _currentInputIndex;

		public int CurrentSequenceIndex { get; set; }
		public List<int> CurrentSequence { get; private set; }
		public int CurrentInputIndex => _currentInputIndex;

		private readonly ILevelsProvider _levelsProvider;

		public GameModel(ILevelsProvider levelsProvider)
		{
			_levelsProvider = levelsProvider;
			CurrentSequence = new List<int>();
		}

		public async UniTask InitializeAsync(CancellationToken cancellationToken)
		{
			await _levelsProvider.InitializeAsync(cancellationToken);
			ResetGame();
		}

		public void ResetGame()
		{
			CurrentSequenceIndex = 0;
			CurrentSequence.Clear();
			_currentInputIndex = 0;
			SequenceIndexChanged?.Invoke();
			SequenceShouldStart?.Invoke();
		}

		public void StartInputPhase()
		{
			_currentInputIndex = 0;
		}

		public void AdvanceInputIndex()
		{
			_currentInputIndex++;
		}

		public void ResetCurrentSequence()
		{
			_currentInputIndex = 0;
		}

	public void CompleteSequence()
	{
		CurrentSequenceIndex++;
		if (CurrentSequenceIndex < _levelsProvider.CurrentLevel.Sequences.Count)
		{
			SequenceIndexChanged?.Invoke();
			SequenceShouldStart?.Invoke();
		}
		else
		{
			SequenceIndexChanged?.Invoke();
			LevelCompleted?.Invoke();
		}
	}

		public void CompleteLevel()
		{
			if (_levelsProvider.CurrentLevelIndex + 1 < _levelsProvider.AllLevels.Count)
			{
				_levelsProvider.SetLevel(_levelsProvider.CurrentLevelIndex + 1);
				ResetGame();
			}
		}
	}
}