using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    public class PlayfieldButton : MonoBehaviour, IPointerDownHandler
    {
        public event Action<PlayfieldButton> OnClicked;

        [SerializeField]
        private Image _image;

        public int ButtonIndex { get; private set; }
        private Color _defaultColor;

        public void Initialize(int buttonIndex)
        {
            ButtonIndex = buttonIndex;
            _defaultColor = _image.color;
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void ResetToDefaultColor()
        {
            _image.color = _defaultColor;
        }

        public async UniTask ShowFeedbackAsync(Color feedbackColor, float duration)
        {
            SetColor(feedbackColor);
            await UniTask.Delay((int)(duration * 1000));
            ResetToDefaultColor();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
    }
}

