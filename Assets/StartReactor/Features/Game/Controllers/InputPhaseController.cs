using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Core;
using StartReactor.Features.Environment;
using StartReactor.Features.Presentation;

namespace StartReactor.Features.Game
{
	public class InputPhaseController
	{
		public enum InputPhaseResult
		{
			Success,
			Failed
		}

		private readonly IPlayfieldView _playfieldView;
		private readonly VisualFeedbackController _visualFeedback;
		private readonly SequenceState _sequenceState;

		private InputPhaseResult _result;
		private bool _isWaitingForInput;

		public InputPhaseController(
			IPlayfieldView playfieldView,
			VisualFeedbackController visualFeedback,
			GameModel gameModel)
		{
			_playfieldView = playfieldView;
			_visualFeedback = visualFeedback;
			_sequenceState = gameModel.SequenceState;
		}

		public void Start()
		{
			_playfieldView.OnButtonClicked += OnButtonClicked;
		}

		public void Stop()
		{
			_playfieldView.OnButtonClicked -= OnButtonClicked;
		}

		public async UniTask<InputPhaseResult> WaitForInput(CancellationToken cancellationToken)
		{
			_isWaitingForInput = true;
			_sequenceState.ResetInput();
			_visualFeedback.ResetAllButtonsToDefault();

			while (_isWaitingForInput && !cancellationToken.IsCancellationRequested)
			{
				await UniTask.Yield();
			}

			return _result;
		}

		private void OnButtonClicked(int buttonIndex)
		{
			if (!_isWaitingForInput)
			{
				return;
			}

			if (!ValidateInput(buttonIndex))
			{
				_result = InputPhaseResult.Failed;
				_isWaitingForInput = false;
				return;
			}

			_sequenceState.AdvanceInput();

			if (_sequenceState.CurrentInputIndex >= _sequenceState.CurrentSequence.Count)
			{
				_visualFeedback.SetAllButtonsCorrect();
				_result = InputPhaseResult.Success;
				_isWaitingForInput = false;
			}
			else
			{
				_visualFeedback.ShowButtonFeedback(buttonIndex);
			}
		}

		private bool ValidateInput(int buttonIndex)
		{
			if (_sequenceState.CurrentInputIndex >= _sequenceState.CurrentSequence.Count)
			{
				return false;
			}

			return _sequenceState.CurrentSequence[_sequenceState.CurrentInputIndex] == buttonIndex;
		}
	}
}