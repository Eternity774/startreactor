using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    /// <summary>
    /// PlayfieldView manages the visual representation of the gameplay grid.
    /// Handles only visual updates - no logic.
    /// </summary>
    public class PlayfieldView : MonoBehaviour, IPlayfieldView
    {
        public event Action<int> OnButtonClicked;

        [SerializeField]
        private Transform _gridContainer;

        private List<PlayfieldButton> _buttons = new List<PlayfieldButton>();
        private Dictionary<int, Color> _defaultColors = new Dictionary<int, Color>();

        public Transform GridContainer => _gridContainer;

        public void InitializeButtons(List<PlayfieldButton> buttons)
        {
            ClearGrid();

            _buttons = buttons;

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i] != null)
                {
                    // Cache default color
                    if (_buttons[i].Image != null)
                    {
                        _defaultColors[i] = _buttons[i].Image.color;
                    }

                    // Subscribe to click events
                    _buttons[i].OnClicked += HandleButtonClicked;
                }
            }
        }

        private void HandleButtonClicked(PlayfieldButton button)
        {
            OnButtonClicked?.Invoke(button.ButtonIndex);
        }

        public void SetButtonHighlight(int buttonIndex, bool highlight, float intensity)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Count || _buttons[buttonIndex] == null)
                return;

            if (_buttons[buttonIndex].Image == null)
                return;

            Color baseColor = _defaultColors.ContainsKey(buttonIndex) 
                ? _defaultColors[buttonIndex] 
                : _buttons[buttonIndex].Image.color;

            _buttons[buttonIndex].Image.color = highlight 
                ? baseColor * intensity 
                : baseColor;
        }

        public void SetButtonColor(int buttonIndex, Color color)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Count || _buttons[buttonIndex] == null)
                return;

            if (_buttons[buttonIndex].Image != null)
            {
                _buttons[buttonIndex].Image.color = color;
            }
        }

        public void SetButtonInteractable(int buttonIndex, bool interactable)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Count || _buttons[buttonIndex] == null)
                return;

            _buttons[buttonIndex].SetInteractable(interactable);
        }

        public void SetAllButtonsInteractable(bool interactable)
        {
            foreach (var button in _buttons)
            {
                if (button != null)
                {
                    button.SetInteractable(interactable);
                }
            }
        }

        public void ShowButtonFeedback(int buttonIndex, Color feedbackColor, float duration)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Count || _buttons[buttonIndex] == null)
                return;

            ShowButtonFeedbackAsync(buttonIndex, feedbackColor, duration).Forget();
        }

        private async UniTaskVoid ShowButtonFeedbackAsync(int buttonIndex, Color feedbackColor, float duration)
        {
            if (buttonIndex < 0 || buttonIndex >= _buttons.Count || _buttons[buttonIndex] == null)
                return;

            Image image = _buttons[buttonIndex].Image;
            if (image == null)
                return;

            Color originalColor = image.color;
            image.color = feedbackColor;

            await UniTask.Delay((int)(duration * 1000));

            if (image != null && _defaultColors.ContainsKey(buttonIndex))
            {
                image.color = _defaultColors[buttonIndex];
            }
        }

        public void ClearGrid()
        {
            // Unsubscribe from events
            foreach (var button in _buttons)
            {
                if (button != null)
                {
                    button.OnClicked -= HandleButtonClicked;
                    Destroy(button.gameObject);
                }
            }

            _buttons.Clear();
            _defaultColors.Clear();
        }

        private void OnDestroy()
        {
            ClearGrid();
        }
    }
}

