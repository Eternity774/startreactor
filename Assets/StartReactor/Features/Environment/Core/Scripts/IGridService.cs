using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;

namespace StartReactor.Features.Environment
{
    public interface IGridService
    {
        UniTask CreateGrid(LevelConfiguration level, IPlayfieldView playfieldView, CancellationToken cancellationToken);
    }
}

