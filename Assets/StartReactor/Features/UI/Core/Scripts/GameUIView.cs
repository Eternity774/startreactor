using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// GameUIView is a MonoBehaviour representing the UI view components in the game.
    /// It provides access to UI elements like score display, round indicator, etc.
    /// </summary>
    public class GameUIView : MonoBehaviour
    {
        [SerializeField]
        private Text _roundText;

        [SerializeField]
        private Text _scoreText;

        [SerializeField]
        private Text _sequenceLengthText;

        [SerializeField]
        private GameObject _gameOverPanel;

        [SerializeField]
        private Button _restartButton;

        [SerializeField]
        private Transform _popupContainer;

        public Text RoundText => _roundText;
        public Text ScoreText => _scoreText;
        public Text SequenceLengthText => _sequenceLengthText;
        public GameObject GameOverPanel => _gameOverPanel;
        public Button RestartButton => _restartButton;
        public Transform PopupContainer => _popupContainer;
    }
}

