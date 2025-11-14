using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StartReactor.Features.Presentation;

namespace StartReactor.Features.Game
{
	public class SequencePlaybackController
	{
		private readonly VisualFeedbackController _visualFeedback;

		public SequencePlaybackController(VisualFeedbackController visualFeedback)
		{
			_visualFeedback = visualFeedback;
		}

		public async UniTask PlaySequence(List<int> sequence, CancellationToken cancellationToken)
		{
			_visualFeedback.SetAllButtonsDisabled();

			foreach (var buttonIndex in sequence)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await _visualFeedback.HighlightSequenceButton(buttonIndex, cancellationToken);
			}
		}
	}
}