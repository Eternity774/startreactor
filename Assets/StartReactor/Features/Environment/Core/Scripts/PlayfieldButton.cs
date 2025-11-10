using System;
using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// PlayfieldButton represents a single button in the gameplay grid.
    /// Handles click events and exposes visual properties.
    /// </summary>
    public class PlayfieldButton : MonoBehaviour
    {
        public event Action<PlayfieldButton> OnClicked;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _image;

        public Button Button => _button;
        public Image Image => _image;
        public int ButtonIndex { get; private set; }

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            if (_button != null)
            {
                _button.onClick.AddListener(HandleClick);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClick);
            }
        }

        public void Initialize(int buttonIndex)
        {
            ButtonIndex = buttonIndex;
        }

        public void SetInteractable(bool interactable)
        {
            if (_button != null)
            {
                _button.interactable = interactable;
            }
        }

        private void HandleClick()
        {
            OnClicked?.Invoke(this);
        }
    }
}

