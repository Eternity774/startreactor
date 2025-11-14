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
        private readonly ILevelsProvider _levelsProvider;

        public GameUIController(
            IControllerFactory controllerFactory,
            GameModel gameModel,
            GameUIView uiView,
            ILevelsProvider levelsProvider)
            : base(controllerFactory)
        {
            _gameModel = gameModel;
            _uiView = uiView;
            _levelsProvider = levelsProvider;
        }

	protected override void OnStart()
	{
		_gameModel.LevelCompleted += OnLevelCompleted;
		_gameModel.SequenceIndexChanged += UpdateUI;
		UpdateUI();
	}

	protected override void OnStop()
	{
		_gameModel.LevelCompleted -= OnLevelCompleted;
		_gameModel.SequenceIndexChanged -= UpdateUI;
	}

        private void OnLevelCompleted()
        {
            int totalSequences = _levelsProvider.CurrentLevel.Sequences.Count;
            _uiView.SetSequenceText($"{totalSequences}/{totalSequences}");
        }

        private void UpdateUI()
        {
            int totalSequences = _levelsProvider.CurrentLevel.Sequences.Count;
            int currentSequenceIndex = _gameModel.CurrentSequenceIndex;
            int currentSequenceNumber = currentSequenceIndex + 1;
            _uiView.SetSequenceText($"{currentSequenceNumber}/{totalSequences}");
        }
    }
}

