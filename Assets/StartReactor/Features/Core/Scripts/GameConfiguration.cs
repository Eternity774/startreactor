using UnityEngine;

namespace StartReactor.Features.Core
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Start Reactor/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Colors")]
        public Color SequenceColor = Color.yellow;
        public Color CorrectColor = Color.green;
        public Color ErrorColor = Color.red;
        
        [Header("Display Settings")]
        [Range(0.5f, 2.0f)]
        public float ButtonDisplayDuration = 1.0f;
        
        [Range(0.1f, 1.0f)]
        public float ButtonDisplayDelay = 0.3f;
        
        [Range(0.1f, 2.0f)]
        [Tooltip("How long each button stays highlighted (yellow) during sequence playback")]
        public float SequenceButtonHighlightDuration = 1.0f;
        
        [Range(0.1f, 2.0f)]
        public float ErrorFlashDuration = 0.5f;
    }
}

