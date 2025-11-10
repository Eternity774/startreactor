using System;
using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// LosePopupView represents the lose popup UI.
    /// Contains a Restart button to restart the current level.
    /// </summary>
    public class LosePopupView : MonoBehaviour, ILosePopupView
    {
        [SerializeField]
        private Button _restartButton;

        public event Action OnRestartClicked;

        private void Awake()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
            }
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveAllListeners();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

