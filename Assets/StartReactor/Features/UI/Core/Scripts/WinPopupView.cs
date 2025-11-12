using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        private bool _wasNextButtonClicked = false;
        
        private void Awake()
        {
            if (_nextButton != null)
            {
                _nextButton.onClick.AddListener(HandleNextClicked);
            }
        }

        private void OnDestroy()
        {
            if (_nextButton != null)
            {
                _nextButton.onClick.RemoveListener(HandleNextClicked);
            }
        }

        public UniTask Show(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            
            return UniTask.CompletedTask;
        }

        public UniTask Hide(CancellationToken cancellationToken)
        {
            gameObject.SetActive(false);
            
            Destroy(gameObject);
            
            return UniTask.CompletedTask;
        }

        public async UniTask WaitForNextButtonClicked(CancellationToken token)
        {
            await UniTask.WaitUntil(() => _wasNextButtonClicked, cancellationToken: token);
        }
        
        private void HandleNextClicked()
        {
            _wasNextButtonClicked = true;
        }
    }
}

