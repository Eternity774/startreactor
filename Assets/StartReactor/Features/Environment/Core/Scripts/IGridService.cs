using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.Environment
{
    public interface IGridService
    {
        UniTask<List<PlayfieldButton>> CreateGrid(LevelConfiguration level, Transform container, CancellationToken cancellationToken);
    }
}

