using System.Collections.Generic;
using UnityEngine;

namespace StartReactor.Features.Core
{
    /// <summary>
    /// LevelsConfig contains a collection of all level configurations.
    /// This is loaded as a single addressable asset, making it easier to manage all levels.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Start Reactor/Levels Config")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField]
        private List<LevelConfiguration> _levels = new List<LevelConfiguration>();

        public List<LevelConfiguration> Levels => _levels;
    }
}

