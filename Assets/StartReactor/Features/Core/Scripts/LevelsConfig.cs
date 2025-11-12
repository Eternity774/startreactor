using System.Collections.Generic;
using UnityEngine;

namespace StartReactor.Features.Core
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Start Reactor/Levels Config")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField]
        private List<LevelConfiguration> _levels = new List<LevelConfiguration>();

        public List<LevelConfiguration> Levels => _levels;
    }
}

