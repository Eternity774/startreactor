using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Game;

namespace StartReactor.Features.Core
{
	public class GameModel
	{
		public event Action LevelCompleted;
		public event Action SequenceStartRequested;
		public event Action SequenceIndexChanged;

		public SequenceState SequenceState { get; }

		private readonly ILevelsProvider _levelsProvider;

		public int CurrentSequenceIndex => SequenceState.CurrentSequenceIndex;

		public GameModel(ILevelsProvider levelsProvider)
		{
			_levelsProvider = levelsProvider;
			SequenceState = new SequenceState();
		}

		public async UniTask Initialize(CancellationToken cancellationToken)
		{
			await _levelsProvider.Initialize(cancellationToken);
			ResetGame();
		}

		public void ResetGame()
		{
			SequenceState.Reset();
			SequenceIndexChanged?.Invoke();
			SequenceStartRequested?.Invoke();
		}

		public void CompleteSequence()
		{
			SequenceState.CurrentSequenceIndex++;
			SequenceIndexChanged?.Invoke();

			if (SequenceState.CurrentSequenceIndex < _levelsProvider.CurrentLevel.Sequences.Count)
			{
				SequenceStartRequested?.Invoke();
			}
			else
			{
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