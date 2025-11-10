using UnityEngine;

namespace StartReactor.Features.Core
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Start Reactor/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Colors")]
        public Color CorrectColor = Color.green;
        
        public Color ErrorColor = Color.red;
        
        [Header("Display Settings")]
        [Range(0.5f, 2.0f)]
        public float ButtonDisplayDuration = 1.0f;
        
        [Range(0.1f, 1.0f)]
        public float ButtonDisplayDelay = 0.3f;
    }
}

