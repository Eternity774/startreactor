using StartReactor.Features.Environment;
using StartReactor.Features.UI;
using UnityEngine;

namespace StartReactor.Features.Core
{
    public class GameView : MonoBehaviour
    {
        public GameUIView UiView => _uiView;
        public GameEnvironmentView GameEnvironmentView => _gameEnvironmentView;

        [SerializeField]
        private GameUIView _uiView;

        [SerializeField]
        private GameEnvironmentView _gameEnvironmentView;
    }
}

