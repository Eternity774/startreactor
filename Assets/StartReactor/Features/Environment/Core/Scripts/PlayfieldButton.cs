using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StartReactor.Features.Environment
{
    public class PlayfieldButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private Image _image;

        private Color _defaultColor;

        public int ButtonIndex { get; private set; }

        public event Action<PlayfieldButton> OnClicked;

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

        public async UniTask ShowColorFeedback(Color color, float duration)
        {
            SetColor(color);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            ResetToDefaultColor();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClicked?.Invoke(this);
        }
    }
}

