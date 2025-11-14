using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StartReactor.Features.Core
{
	public interface ILevelsProvider
	{
		LevelConfiguration CurrentLevel { get; }
		List<LevelConfiguration> AllLevels { get; }
		int CurrentLevelIndex { get; }

		UniTask InitializeAsync(CancellationToken cancellationToken);
		void SetLevel(int levelIndex);
	}
}


