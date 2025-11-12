using UnityEngine;

namespace StartReactor.Features.Environment
{
    public interface IGridLayoutConfigurator
    {
        void ConfigureGridLayout(Vector2Int gridSize, Transform container);
    }
}

