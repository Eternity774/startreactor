using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StartReactor.Features.Core
{
	public class LevelsProvider : ILevelsProvider
	{
		private LevelConfiguration _currentLevel;
		private List<LevelConfiguration> _levels;
		private bool _isInitialized;

		public LevelConfiguration CurrentLevel => _currentLevel;
		public List<LevelConfiguration> AllLevels => _levels;
		public int CurrentLevelIndex { get; private set; }

		public async UniTask InitializeAsync(CancellationToken cancellationToken)
		{
			if (!_isInitialized)
			{
				_levels = await LoadAllLevelsAsync(cancellationToken);

				cancellationToken.ThrowIfCancellationRequested();

				if (_levels.Count == 0)
				{
					throw new System.Exception("No levels found. At least one level must be configured.");
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
			var handle =
				Addressables.LoadAssetAsync<LevelsConfig>(AddressableKeys.LevelsConfig);

			try
			{
				await handle.ToUniTask(cancellationToken: cancellationToken);

				if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
				{
					var levelsConfig = handle.Result;

					var levels = levelsConfig.Levels
						.Where(level => level != null)
						.ToList();

					return levels;
				}
				else
				{
					Debug.LogError($"Failed to load LevelsConfig. Status: {handle.Status}");
					Addressables.Release(handle);
					return new List<LevelConfiguration>();
				}
			}
			catch (System.OperationCanceledException)
			{
				Addressables.Release(handle);
				throw;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error loading LevelsConfig: {ex.Message}");
				Addressables.Release(handle);
				return new List<LevelConfiguration>();
			}
		}
	}
}