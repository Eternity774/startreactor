using System;
using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// WinPopupView represents the win popup UI.
    /// Contains a Next button to proceed to the next level.
    /// </summary>
    public class WinPopupView : MonoBehaviour, IWinPopupView
    {
        [SerializeField]
        private Button _nextButton;

        public event Action OnNextClicked;

        private void Awake()
        {
            if (_nextButton != null)
            {
                _nextButton.onClick.AddListener(HandleNextClicked);
            }
        }

        private void HandleNextClicked()
        {
            OnNextClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if (_nextButton != null)
            {
                _nextButton.onClick.RemoveListener(HandleNextClicked);
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

