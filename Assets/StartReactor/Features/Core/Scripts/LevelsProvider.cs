using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.Core
{
	public class LevelsProvider : ILevelsProvider
	{
		private readonly ResourcesProvider _resourcesProvider;
		private LevelConfiguration _currentLevel;
		private List<LevelConfiguration> _levels;
		private bool _isInitialized;

		public LevelConfiguration CurrentLevel => _currentLevel;
		public List<LevelConfiguration> AllLevels => _levels;
		public int CurrentLevelIndex { get; private set; }

		public LevelsProvider(ResourcesProvider resourcesProvider)
		{
			_resourcesProvider = resourcesProvider;
		}

		public async UniTask Initialize(CancellationToken cancellationToken)
		{
			if (!_isInitialized)
			{
				_levels = await LoadAllLevelsAsync(cancellationToken);

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
			else
			{
				_currentLevel = _levels[CurrentLevelIndex];
			}
		}

		public void SetLevel(int levelIndex)
		{
			CurrentLevelIndex = levelIndex;
			_currentLevel = _levels[levelIndex];
		}

		private async UniTask<List<LevelConfiguration>> LoadAllLevelsAsync(CancellationToken cancellationToken)
		{
			var levelsConfig = await _resourcesProvider.LoadAsync<LevelsConfig>(AddressableKeys.LevelsConfig, cancellationToken);

			return levelsConfig.Levels;
		}
	}
}