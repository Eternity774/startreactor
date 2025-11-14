using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.Environment
{
	public interface IGridFactory
	{
		UniTask<List<PlayfieldButton>> CreateGridAsync(
			LevelConfiguration level,
			Transform container,
			CancellationToken cancellationToken);
	}
}