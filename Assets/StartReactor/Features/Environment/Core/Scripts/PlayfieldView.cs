using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartReactor.Features.Environment
{
    public class PlayfieldView : MonoBehaviour, IPlayfieldView
    {
        public event Action<int> OnButtonClicked;

        [SerializeField]
        private Transform _gridContainer;

        private List<PlayfieldButton> _buttons = new();

        public Transform GridContainer => _gridContainer;

        public void InitializeButtons(List<PlayfieldButton> buttons)
        {
            ClearGrid();
            
            _buttons = buttons;
            foreach (var button in _buttons)
            {
                button.OnClicked += HandleButtonClicked;
            }
        }

        private void HandleButtonClicked(PlayfieldButton button)
        {
            OnButtonClicked?.Invoke(button.ButtonIndex);
        }

        public void SetButtonColor(int buttonIndex, Color color)
        {
            _buttons[buttonIndex].SetColor(color);
        }

        public void SetAllButtonsColor(Color color)
        {
            foreach (var button in _buttons)
            {
                button.SetColor(color);
            }
        }

        public void ResetAllButtonsToDefaultColor()
        {
            foreach (var button in _buttons)
            {
                button.ResetToDefaultColor();
            }
        }

        public void ShowButtonFeedback(int buttonIndex, Color feedbackColor, float duration)
        {
            _buttons[buttonIndex].ShowColorFeedback(feedbackColor, duration).Forget();
        }

        public void ClearGrid()
        {
            foreach (var button in _buttons)
            {
                button.OnClicked -= HandleButtonClicked;
                Destroy(button.gameObject);
            }

            _buttons.Clear();
        }

        private void OnDestroy()
        {
            ClearGrid();
        }
    }
}

