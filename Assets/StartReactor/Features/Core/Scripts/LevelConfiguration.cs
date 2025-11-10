using System.Collections.Generic;
using UnityEngine;

namespace StartReactor.Features.Core
{
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Start Reactor/Level Configuration")]
    public class LevelConfiguration : ScriptableObject
    {
        public Vector2Int GridSize;
        public List<int> Sequences;
    }
}

