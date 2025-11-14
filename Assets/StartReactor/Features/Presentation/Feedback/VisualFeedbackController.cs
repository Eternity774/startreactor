using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;

namespace StartReactor.Features.Presentation
{
	public class VisualFeedbackController
	{
		private readonly IPlayfieldView _playfieldView;
		private GameConfiguration _gameConfiguration;

		public VisualFeedbackController(IPlayfieldView playfieldView)
		{
			_playfieldView = playfieldView;
		}

		public void Initialize(GameConfiguration gameConfiguration)
		{
			_gameConfiguration = gameConfiguration;
		}

		public void SetAllButtonsDisabled()
		{
			_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);
		}

		public void ResetAllButtonsToDefault()
		{
			_playfieldView.ResetAllButtonsToDefaultColor();
		}

		public void SetAllButtonsCorrect()
		{
			_playfieldView.SetAllButtonsColor(_gameConfiguration.CorrectColor);
		}

		public void ShowButtonFeedback(int buttonIndex)
		{
			_playfieldView.ShowButtonFeedback(buttonIndex, _gameConfiguration.CorrectColor, _gameConfiguration.ButtonColorFeedbackDuration);
		}

		public async UniTask ShowErrorFlash(CancellationToken cancellationToken)
		{
			_playfieldView.SetAllButtonsColor(_gameConfiguration.ErrorColor);
			
			await UniTask.Delay(TimeSpan.FromSeconds(_gameConfiguration.ErrorFlashDuration), cancellationToken: cancellationToken);
			
			_playfieldView.SetAllButtonsColor(_gameConfiguration.DisabledColor);
		}

		public async UniTask HighlightSequenceButton(int buttonIndex, CancellationToken cancellationToken)
		{
			_playfieldView.SetButtonColor(buttonIndex, _gameConfiguration.SequenceColor);

			await UniTask.Delay(
				TimeSpan.FromSeconds(_gameConfiguration.SequenceButtonHighlightDuration),
				cancellationToken: cancellationToken);

			_playfieldView.SetButtonColor(buttonIndex, _gameConfiguration.DisabledColor);

			await UniTask.Delay(
				TimeSpan.FromSeconds(_gameConfiguration.ButtonDisplayDelay + _gameConfiguration.SequenceButtonPostFeedbackDelay),
				cancellationToken: cancellationToken);
		}
	}
}

