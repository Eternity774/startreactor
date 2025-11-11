using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using StartReactor.Features.Core;
using UnityEngine;

namespace StartReactor.Features.UI
{
    public class GameUIController : ControllerBase
    {
        private readonly GameModel _gameModel;
        private readonly GameUIView _uiView;

        public GameUIController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            GameUIView uiView)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _uiView = uiView;
        }

        protected override void OnStart()
        {
            _gameModel.SequenceGenerated += OnSequenceGenerated;
            _gameModel.LevelCompleted += OnLevelCompleted;

            UpdateUI();
        }

        protected override void OnStop()
        {
            _gameModel.SequenceGenerated -= OnSequenceGenerated;
            _gameModel.LevelCompleted -= OnLevelCompleted;
        }

        private void OnSequenceGenerated(System.Collections.Generic.List<int> sequence)
        {
            UpdateUI();
        }

        private void OnLevelCompleted()
        {
            int totalSequences = _gameModel.CurrentLevel.Sequences.Count;
            _uiView.SetSequenceText($"{totalSequences}/{totalSequences}");
        }

        private void UpdateUI()
        {
            int totalSequences = _gameModel.CurrentLevel.Sequences.Count;
            int currentSequenceIndex = _gameModel.CurrentSequenceIndex;
            int currentSequenceNumber = currentSequenceIndex + 1;
            _uiView.SetSequenceText($"{currentSequenceNumber}/{totalSequences}");
        }
    }
}

