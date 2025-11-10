using UnityEngine;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// Interface for configuring grid layout.
    /// </summary>
    public interface IGridLayoutConfigurator
    {
        void ConfigureGridLayout(Vector2Int gridSize, Transform container);
    }
}

