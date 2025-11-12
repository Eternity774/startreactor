using TMPro;
using UnityEngine;

namespace StartReactor.Features.UI
{
    public class GameUIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _sequenceLengthText;

        [SerializeField]
        private Transform _popupContainer;

        public Transform PopupContainer => _popupContainer;

        public void SetSequenceText(string text)
        {
            _sequenceLengthText.text = text;
        }
    }
}

