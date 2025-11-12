using UnityEngine;

namespace StartReactor.Features.Core
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Start Reactor/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Colors")]
        [SerializeField]
        private Color _disabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        [SerializeField]
        private Color _sequenceColor = Color.yellow;
        [SerializeField]
        private Color _correctColor = Color.green;
        [SerializeField]
        private Color _errorColor = Color.red;

        [Header("Display Settings")]
        [SerializeField]
        private float _buttonColorFeedbackDuration = 1f;
        [SerializeField]
        private float _buttonDisplayDelay = 0.3f;
        [SerializeField]
        private float _sequenceButtonHighlightDuration = 1.0f;
        [SerializeField]
        private float _errorFlashDuration = 0.5f;
        [SerializeField]
        private float _sequenceWinDelay = 1f;
        [SerializeField]
        private float _sequenceCompletionDelay = 0.5f;
        [SerializeField]
        private float _sequenceButtonPostFeedbackDelay = 0.1f;
        [SerializeField]
        private float _gridCreationDelay = 1.5f;

        public Color DisabledColor => _disabledColor;
        public Color SequenceColor => _sequenceColor;
        public Color CorrectColor => _correctColor;
        public Color ErrorColor => _errorColor;
        public float ButtonColorFeedbackDuration => _buttonColorFeedbackDuration;
        public float ButtonDisplayDelay => _buttonDisplayDelay;
        public float SequenceButtonHighlightDuration => _sequenceButtonHighlightDuration;
        public float ErrorFlashDuration => _errorFlashDuration;
        public float SequenceWinDelay => _sequenceWinDelay;
        public float SequenceCompletionDelay => _sequenceCompletionDelay;
        public float SequenceButtonPostFeedbackDelay => _sequenceButtonPostFeedbackDelay;
        public float GridCreationDelay => _gridCreationDelay;
    }
}

