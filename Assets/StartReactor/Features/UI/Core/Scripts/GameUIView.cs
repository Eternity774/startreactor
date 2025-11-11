using TMPro;
using UnityEngine;

namespace StartReactor.Features.UI
{
    /// <summary>
    /// GameUIView is a MonoBehaviour representing the UI view components in the game.
    /// Pure view - no logic, just methods to update UI elements.
    /// </summary>
    public class GameUIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _sequenceLengthText;

        [SerializeField]
        private Transform _popupContainer;

        public Transform PopupContainer => _popupContainer;

        public void SetSequenceText(string text)
        {
            if (_sequenceLengthText != null)
            {
                _sequenceLengthText.text = text;
            }
        }
    }
}

